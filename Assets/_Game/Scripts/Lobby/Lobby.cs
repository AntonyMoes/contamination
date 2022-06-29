using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Network;
using LiteNetLib.Utils;
using UnityEngine;
using Guid = _Game.Scripts.Utils.Guid;

namespace _Game.Scripts.Lobby {
    public class Lobby<TRoomSettings> : IDisposable where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        private readonly IPeerCollection _clientPeers;
        private readonly Action<TRoomSettings, IReadOnlyCollection<LobbyUser>> _gameStarter;
        private readonly List<LobbyUser> _users = new List<LobbyUser>();
        private readonly List<Room<TRoomSettings>> _rooms = new List<Room<TRoomSettings>>();

        public Lobby(IPeerCollection peers, Action<TRoomSettings, IReadOnlyCollection<LobbyUser>> gameStarter) {
            _clientPeers = peers;
            _gameStarter = gameStarter;
            _clientPeers.GetReceiveEvent<LobbyTransitMessage>().Subscribe(OnLobbyTransit);
            _clientPeers.GetReceiveEvent<RoomTransitMessage>().Subscribe(OnRoomTransit);
            _clientPeers.GetReceiveEvent<RoomUpdateMessage<TRoomSettings>>().Subscribe(OnRoomUpdate);
            _clientPeers.GetReceiveEvent<GameStartMessage>().Subscribe(OnGameStart);
            _clientPeers.OnPeerConnection.Subscribe(OnPeerConnection);
        }

        private void OnPeerConnection(IPeer peer, bool connected) {
            if (!connected) {
                HandleDisconnect(peer);
            }
        }

        private void OnLobbyTransit(LobbyTransitMessage message, IPeer peer) {
            Debug.Log("Got lobby connection request!");
            if (!message.Join) {
                HandleDisconnect(peer, message);
                return;
            }

            if (_users.Any(user => user.Peer == peer)) {
                // TODO no exception?
                throw new ArgumentException("Peer is already in the lobby", nameof(peer));
            }

            var reason = _users.Any(user => user.Name == message.Name)
                ? LobbyTransitResponseMessage<TRoomSettings>.ERefuseReason.NameTaken
                : LobbyTransitResponseMessage<TRoomSettings>.ERefuseReason.None;

            var accepted = reason == LobbyTransitResponseMessage<TRoomSettings>.ERefuseReason.None;

            LobbyUser newUser = null;
            if (accepted) {
                newUser = new LobbyUser(Guid.New, peer, message.Name);
                _users.Add(newUser);
            }

            peer.SendMessage(new LobbyTransitResponseMessage<TRoomSettings> {
                RefuseReason = reason,
                UserId = newUser?.Id,
                LobbyData = accepted
                    ? GetData()
                    : null
            }, message);

            if (accepted) {
                SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.CreateOrUpdate, newUser, except: peer);
            }
        }

        private void HandleDisconnect(IPeer peer, Message respondTo = null) {
            var user = _users.FirstOrDefault(u => u.Peer == peer);
            if (user == null) {
                return;
            }

            _users.Remove(user);
            var room = _rooms.FirstOrDefault(r => r.Users.Contains(user));
            if (room != null) {
                var stillExists = room.RemoveUser(user);
                if (!stillExists) {
                    _rooms.Remove(room);
                }
            }

            if (respondTo != null) {
                peer.SendMessage(new LobbyTransitResponseMessage<TRoomSettings> {
                    RefuseReason = LobbyTransitResponseMessage<TRoomSettings>.ERefuseReason.None
                }, respondTo);
            }

            if (room != null) {
                SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.Delete, room: room, except: peer);
            }

            SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.Delete, user, except: peer);
        }

        private void OnRoomTransit(RoomTransitMessage message, IPeer peer) {
            var user = _users.FirstOrDefault(u => u.Peer == peer);
            var room = _rooms.FirstOrDefault(r => r.Id == message.RoomId);
            var oldRoom = message.Join
                ? _rooms.FirstOrDefault(r => r.Users.Contains(user))
                : room;
            var newRoom = message.Join ? room : null;

            var oldRoomStillExists = true;
            RoomTransitResponseMessage<TRoomSettings>.ERefuseReason reason;
            if (user == null) {
                reason = RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.NotAuthorized;
            } else if (message.Join && newRoom == null || !message.Join && oldRoom == null) {
                reason = RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.NoRoom;
            } else if (message.Join && !newRoom.CheckPassword(message.Password)) {
                reason = RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.WrongPassword;
            } else if (message.Join && !newRoom.TryAddUser(user)) {
                reason = RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.NotAllowed;
            } else {
                if (oldRoom != null) {
                    oldRoomStillExists = oldRoom.RemoveUser(user);
                    if (!oldRoomStillExists) {
                        _rooms.Remove(oldRoom);
                    }
                }

                reason = RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.None;
            }

            peer.SendMessage(new RoomTransitResponseMessage<TRoomSettings> {
                LobbyData = GetData(),
                RefuseReason = reason
            }, message);

            var accepted = reason == RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.None;
            if (!accepted) {
                return;
            }

            if (oldRoom != null) {
                SendLobbyUpdate(
                    !oldRoomStillExists
                        ? LobbyUpdateMessage<TRoomSettings>.EActionType.Delete
                        : LobbyUpdateMessage<TRoomSettings>.EActionType.CreateOrUpdate,
                    room: oldRoom, except: peer);
            }

            if (newRoom != null) {
                SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.CreateOrUpdate, room: newRoom, except: peer);
            }
        }

        private void OnRoomUpdate(RoomUpdateMessage<TRoomSettings> message, IPeer peer) {
            var user = _users.FirstOrDefault(u => u.Peer == peer);

            RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason reason;
            Room<TRoomSettings> updatedRoom = null;
            if (user == null) {
                reason = RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.NotAuthorized;
            } else if (string.IsNullOrEmpty(message.RoomId)) {
                try {
                    updatedRoom = new Room<TRoomSettings>(user, message.Password, message.Settings);
                    _rooms.Add(updatedRoom);
                    reason = RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.None;
                } catch (ArgumentException) {
                    reason = RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.InvalidSettings;
                }
            } else if (!(_rooms.FirstOrDefault(r => r.Id == message.RoomId) is { } room)) {
                reason = RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.NoRoom;
            } else if (!room.CanMakeUpdate(user, message.Settings)) {
                reason = RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.NotAllowed;
            } else {
                if (message.UpdatePassword) {
                    room.UpdatePassword(message.Password);
                }

                if (message.UpdateSettings) {
                    room.MakeUpdate(message.Settings);
                }

                if (message.UpdatePassword || message.UpdateSettings) {
                    updatedRoom = room;
                }

                reason = RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.None;
            }

            peer.SendMessage(new RoomUpdateResponseMessage<TRoomSettings> {
                LobbyData = GetData(),
                RefuseReason = reason,
                RoomId = updatedRoom?.Id
            }, message);

            var accepted = reason == RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.None;
            if (accepted && updatedRoom != null) {
                SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.CreateOrUpdate, room: updatedRoom, except: peer);
            }
        }

        private void OnGameStart(GameStartMessage message, IPeer peer) {
            var user = _users.FirstOrDefault(u => u.Peer == peer);

            Room<TRoomSettings> roomToRemove = null;
            GameStartResponseMessage.ERefuseReason reason;
            if (user == null) {
                reason = GameStartResponseMessage.ERefuseReason.NotAuthorized;
            } else if (!(_rooms.FirstOrDefault(r => r.Users.Any(u => u.Peer == peer)) is { } room)) {
                reason = GameStartResponseMessage.ERefuseReason.NoRoom;
            } else if (!room.CanStartGame(user)) {
                reason = GameStartResponseMessage.ERefuseReason.NotAllowed;
            } else {
                roomToRemove = room;
                reason = GameStartResponseMessage.ERefuseReason.None;
            }

            peer.SendMessage(new GameStartResponseMessage {
                RefuseReason = reason
            }, message);

            if (reason == GameStartResponseMessage.ERefuseReason.None) {
                var roomPeers = roomToRemove.Users.Select(u => u.Peer).ToArray();
                foreach (var roomPeer in roomPeers.Where(p => p != peer)) {
                    roomPeer.SendMessage(new ServerGameStartMessage());
                }

                _rooms.Remove(roomToRemove);
                SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.Delete, room: roomToRemove, except: roomPeers);

                foreach (var lobbyUser in roomToRemove.Users) {
                    _users.Remove(lobbyUser);
                    SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType.Delete, lobbyUser, except: roomPeers);
                }

                _gameStarter(roomToRemove.GetData().Settings, roomToRemove.Users);
            }
        }

        private void SendLobbyUpdate(LobbyUpdateMessage<TRoomSettings>.EActionType actionType, LobbyUser user = null,
            Room<TRoomSettings> room = null, params IPeer[] except) {
            if (!((user == null) ^ (room == null))) {
                throw new ArgumentException("Either user or room should be null, but not both");
            }

            var contentType = user != null
                ? LobbyUpdateMessage<TRoomSettings>.EContentType.User
                : LobbyUpdateMessage<TRoomSettings>.EContentType.Room;

            foreach (var lobbyUser in _users.Where(lobbyUser => !except.Contains(lobbyUser.Peer))) {
                lobbyUser.Peer.SendMessage(new LobbyUpdateMessage<TRoomSettings> {
                    ActionType = actionType,
                    ContentType = contentType,
                    User = user?.GetData(),
                    Room = room?.GetData()
                });
            }
        }

        private Data GetData() {
            return new Data {
                Users = _users.Select(user => user.GetData()).ToArray(),
                Rooms = _rooms.Select(room => room.GetData()).ToArray()
            };
        }

        public void Dispose() {
            _clientPeers.GetReceiveEvent<LobbyTransitMessage>().Unsubscribe(OnLobbyTransit);
            _clientPeers.GetReceiveEvent<RoomTransitMessage>().Unsubscribe(OnRoomTransit);
            _clientPeers.GetReceiveEvent<RoomUpdateMessage<TRoomSettings>>().Unsubscribe(OnRoomUpdate);
            _clientPeers.GetReceiveEvent<GameStartMessage>().Unsubscribe(OnGameStart);
            _clientPeers.OnPeerConnection.Unsubscribe(OnPeerConnection);
        }

        public class Data : INetSerializable {
            public LobbyUser.Data[] Users;
            public Room<TRoomSettings>.Data[] Rooms;

            public void Serialize(NetDataWriter writer) {
                writer.PutArray(Users);
                writer.PutArray(Rooms);
            }

            public void Deserialize(NetDataReader reader) {
                Users = reader.GetArray<LobbyUser.Data>();
                Rooms = reader.GetArray<Room<TRoomSettings>.Data>();
            }
        }
    }
}