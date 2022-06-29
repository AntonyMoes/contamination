using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib.Utils;
using Guid = _Game.Scripts.Utils.Guid;

namespace _Game.Scripts.Lobby {
    public class Room<TRoomSettings> where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        private readonly List<LobbyUser> _users = new List<LobbyUser>();
        private string _password;
        private readonly TRoomSettings _roomSettings;

        public readonly string Id;
        public IReadOnlyCollection<LobbyUser> Users => _users;

        public Room(LobbyUser host, string password, TRoomSettings initialSettings) {
            Id = Guid.New;
            _password = password;
            _roomSettings = initialSettings;
            if (!TryAddUser(host)) {
                throw new ArgumentException("Could not create a valid room with such settings", nameof(initialSettings));
            }
        }

        public bool CheckPassword(string password) {
            return string.IsNullOrEmpty(_password) || password == _password;
        }

        public bool TryAddUser(LobbyUser user) {
            if (!_roomSettings.CanJoin(user.GetData())) {
                return false;
            }

            _users.Add(user);
            _roomSettings.Join(user.GetData());
            return true;
        }

        public bool RemoveUser(LobbyUser user) {
            _users.Remove(user);
            _roomSettings.Leave(user.Id);
            return _users.Count > 0;
        }

        public bool CanMakeUpdate(LobbyUser user, TRoomSettings update) {
            return _users.Contains(user) && _roomSettings.CanMakeUpdate(user.GetData(), update);
        }

        public void MakeUpdate(TRoomSettings update) {
            _roomSettings.MakeUpdate(update);
        }

        public bool CanStartGame(LobbyUser user) {
            return _users.Contains(user) && _roomSettings.CanStartGame(user.GetData());
        }

        public Data GetData() {
            return new Data {
                Id = Id,
                Users = _users.Select(u => u.GetData()).ToArray(),
                HasPassword = !string.IsNullOrEmpty(_password),
                Settings = _roomSettings
            };
        }

        public class Data : INetSerializable {
            public string Id;
            public LobbyUser.Data[] Users;
            public bool HasPassword;
            public TRoomSettings Settings;
            
            public void Serialize(NetDataWriter writer) {
                writer.Put(Id);
                writer.PutArray(Users);
                writer.Put(HasPassword);
                writer.Put(Settings);
            }

            public void Deserialize(NetDataReader reader) {
                Id = reader.GetString();
                Users = reader.GetArray<LobbyUser.Data>();
                HasPassword = reader.GetBool();
                Settings = reader.Get<TRoomSettings>();
            }
        }

        public void UpdatePassword(string password) {
            _password = password;
        }
    }
}
