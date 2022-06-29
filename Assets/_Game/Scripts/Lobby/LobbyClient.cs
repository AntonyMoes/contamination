using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Network;
using GeneralUtils;
using GeneralUtils.Processes;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.Lobby {
    public class LobbyClient<TRoomSettings>where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        private readonly IPeer _serverPeer;
        private readonly List<Process> _sendProcesses = new List<Process>();
        private readonly StateSwitcher<EState> _stateSwitcher = new StateSwitcher<EState>(EState.NotConnected);

        private Action _onLobbyUpdate;
        public Event OnLobbyUpdate { get; }
        private Action _onServerGameStart;
        public Event OnServerGameStart { get; }

        public EState State => _stateSwitcher.State;
        public bool RequestInProcess => _sendProcesses.Count != 0;
        public Lobby<TRoomSettings>.Data LobbyData { get; private set; }
        public string Id { get; private set; }
        public string RoomId { get; private set; }


        public LobbyClient(IPeer serverPeer) {
            _serverPeer = serverPeer;
            OnLobbyUpdate = new Event(out _onLobbyUpdate);
            OnServerGameStart = new Event(out _onServerGameStart);
        }

        public void TryJoinLobby(string name, Action<bool> onDone) {
            _stateSwitcher.CheckState(EState.NotConnected);
            TryTransitLobby(true, onDone, name);
        }

        public void TryLeaveLobby(Action<bool> onDone) {
            _stateSwitcher.CheckState(EState.Connected, EState.InRoom);
            TryTransitLobby(false, onDone);
        }

        private void TryTransitLobby(bool join, Action<bool> onDone, string name = "") {
            Send<LobbyTransitMessage, LobbyTransitResponseMessage<TRoomSettings>>(new LobbyTransitMessage {
                Join = join,
                Name = name
            }, LobbyTransitResponseHandler);

            void LobbyTransitResponseHandler(LobbyTransitResponseMessage<TRoomSettings> message, IPeer _) {
                if (message.RefuseReason == LobbyTransitResponseMessage<TRoomSettings>.ERefuseReason.None) {
                    if (join) {
                        _stateSwitcher.CheckAndSwitchState(EState.Connected, EState.NotConnected);

                        LobbyData = message.LobbyData;
                        Id = message.UserId;

                        EnableUpdatesSubscription(true);
                    } else {
                        HandleDisconnect(EState.Connected, EState.InRoom);
                    }

                    onDone?.Invoke(true);
                } else {
                    onDone?.Invoke(false);
                }
            }
        }

        public void TryJoinRoom(string id, string password, Action<bool> onDone) {
            _stateSwitcher.CheckState(EState.Connected, EState.InRoom);
            TryTransitRoom(id, password, true, joined => {
                if (joined) {
                    EnableGameStartSubscription(true);
                }

                onDone?.Invoke(joined);
            });
        }

        public void TryLeaveRoom(Action<bool> onDone) {
            _stateSwitcher.CheckState(EState.InRoom);
            TryTransitRoom(RoomId, null, false, left => {
                if (left) {
                    EnableGameStartSubscription(false);
                }

                onDone?.Invoke(left);
            });
        }

        private void TryTransitRoom(string id, string password, bool join, Action<bool> onDone) {
            Send<RoomTransitMessage, RoomTransitResponseMessage<TRoomSettings>>(new RoomTransitMessage {
                RoomId = id,
                Join = join,
                Password = password
            }, RoomTransitResponseHandler);

            void RoomTransitResponseHandler(RoomTransitResponseMessage<TRoomSettings> message, IPeer _) {
                if (message.RefuseReason == RoomTransitResponseMessage<TRoomSettings>.ERefuseReason.None) {
                    if (join) {
                        _stateSwitcher.CheckAndSwitchState(EState.InRoom, EState.Connected, EState.InRoom);
                        RoomId = id;
                    } else {
                        _stateSwitcher.CheckAndSwitchState(EState.Connected, EState.InRoom);
                        RoomId = null;
                    }

                    onDone?.Invoke(true);
                } else {
                    onDone?.Invoke(false);
                }
            }
        }

        public void TryUpdateRoom(string id, string password, TRoomSettings settings, Action<bool> onDone) {
            var create = id == null;
            var message = create
                ? new RoomUpdateMessage<TRoomSettings> {
                    RoomId = null,
                    Password = password,
                    Settings = settings
                }
                : new RoomUpdateMessage<TRoomSettings> {
                    RoomId = id,
                    UpdateSettings = true,
                    Settings = settings,
                };

            Send<RoomUpdateMessage<TRoomSettings>, RoomUpdateResponseMessage<TRoomSettings>>(message, RoomTransitResponseHandler);

            void RoomTransitResponseHandler(RoomUpdateResponseMessage<TRoomSettings> response, IPeer _) {
                if (response.RefuseReason == RoomUpdateResponseMessage<TRoomSettings>.ERefuseReason.None) {
                    if (create) {
                        _stateSwitcher.CheckAndSwitchState(EState.InRoom, EState.Connected);
                        RoomId = response.RoomId;
                    }

                    onDone?.Invoke(true);
                } else {
                    onDone?.Invoke(false);
                }
            }
        }

        private void OnServerGameStartMessage(ServerGameStartMessage _, IPeer __) {
            HandleDisconnect(EState.InRoom);
            _onServerGameStart();
        }

        public void TryStartGame(Action<bool> onDone) {
            _stateSwitcher.CheckState(EState.InRoom);
            Send<GameStartMessage, GameStartResponseMessage>(new GameStartMessage(), GameStartResponseHandler);

            void GameStartResponseHandler(GameStartResponseMessage message, IPeer _) {
                var success = message.RefuseReason == GameStartResponseMessage.ERefuseReason.None;
                if (success) {
                    HandleDisconnect(EState.InRoom);
                }

                onDone?.Invoke(success);
            }
        }

        private void HandleDisconnect(params EState[] expected) {
            if (State == EState.InRoom) {
                EnableGameStartSubscription(false);
            }

            _stateSwitcher.CheckAndSwitchState(EState.NotConnected, expected);

            LobbyData = null;
            Id = null;
            RoomId = null;

            EnableUpdatesSubscription(false);
        }

        private void EnableGameStartSubscription(bool enabled) {
            var @event = _serverPeer.GetReceiveEvent<ServerGameStartMessage>();
            if (enabled) {
                @event.Subscribe(OnServerGameStartMessage);
            } else {
                @event.Unsubscribe(OnServerGameStartMessage);
            }
        }

        private void EnableUpdatesSubscription(bool enabled) {
            var @event = _serverPeer.GetReceiveEvent<LobbyUpdateMessage<TRoomSettings>>();
            if (enabled) {
                @event.Subscribe(LobbyUpdateHandler);
            } else {
                @event.Unsubscribe(LobbyUpdateHandler);
            }
        }

        private void LobbyUpdateHandler(LobbyUpdateMessage<TRoomSettings> message, IPeer _) {
            if (message.ContentType == LobbyUpdateMessage<TRoomSettings>.EContentType.Room) {
                var room = LobbyData.Rooms.FirstOrDefault(r => r.Id == message.Room.Id);
                if (message.ActionType == LobbyUpdateMessage<TRoomSettings>.EActionType.CreateOrUpdate) {
                    if (room == null) {
                        LobbyData.Rooms = LobbyData.Rooms.Append(message.Room).ToArray();
                    } else {
                        LobbyData.Rooms[LobbyData.Rooms.IndexOf(room)] = message.Room;
                    }
                } else if (message.ActionType == LobbyUpdateMessage<TRoomSettings>.EActionType.Delete && room != null) {
                    LobbyData.Rooms = LobbyData.Rooms.Where(r => r != room).ToArray();
                } else {
                    return;
                }
            } else {
                var user = LobbyData.Users.FirstOrDefault(u => u.Id == message.User.Id);
                if (message.ActionType == LobbyUpdateMessage<TRoomSettings>.EActionType.CreateOrUpdate && user == null) {
                        LobbyData.Users = LobbyData.Users.Append(message.User).ToArray();
                } else if (message.ActionType == LobbyUpdateMessage<TRoomSettings>.EActionType.Delete && user != null) {
                        LobbyData.Users = LobbyData.Users.Where(u => u != user).ToArray();
                } else {
                    return;
                }
            }

            _onLobbyUpdate();
        }

        public void Stop() {
            if (State != EState.NotConnected) {
                TryTransitLobby(false, null);
            }

            foreach (var process in _sendProcesses) {
                process.TryAbort();
            }

            _sendProcesses.Clear();

            HandleDisconnect(EState.NotConnected, EState.Connected, EState.InRoom);
        }

        private void Send<TMessage, TResponse>(TMessage message, Action<TResponse, IPeer> responseHandler)
            where TMessage : Message, new()
            where TResponse : Message, new() {
            // Debug.Log($"Sending message of type {typeof(TMessage)} and waiting for response of type {typeof(TResponse)}");
            var sendProcess = _serverPeer.SendWithResponse<TMessage, TResponse>(message, null, HandlerWrapper);
            _sendProcesses.Add(sendProcess);

            void HandlerWrapper(TResponse response, IPeer peer) {
                // Debug.Log($"Got response of type {typeof(TResponse)} for a message of type {typeof(TMessage)}");
                if (response is BaseLobbyResponseMessage<TRoomSettings> baseResponse) {
                    LobbyData = baseResponse.LobbyData;
                }

                responseHandler(response, peer);
            }
        }

        public enum EState {
            NotConnected,
            Connected,
            InRoom
        }
    }
}
