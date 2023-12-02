using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Lobby;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Network {
    public class RoomSettings : IRoomSettings<RoomSettings> {
        public const int MaxUsers = 2;

        public List<LobbyUser.Data> Users = new List<LobbyUser.Data>();
        public string HostId;

        public EUpdateType UpdateType;

        public bool CanJoin(LobbyUser.Data user) {
            return Users.Count < MaxUsers;
        }

        public void Join(LobbyUser.Data user) {
            Users.Add(user);
            if (string.IsNullOrEmpty(HostId)) {
                HostId = user.Id;
            }
        }

        public void Leave(string userId) {
            var data = Users.FirstOrDefault(d => d.Id == userId);
            if (data == null) {
                return;
            }

            Users.Remove(data);
            if (data.Id == HostId) {
                HostId = Users.FirstOrDefault() is { } newHost
                    ? newHost.Id
                    : null;
            }
        }

        public bool CanMakeUpdate(LobbyUser.Data user, RoomSettings update) {
            return user.Id == HostId;
        }

        public void MakeUpdate(RoomSettings update) {
            switch (update.UpdateType) {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool CanStartGame(LobbyUser.Data user) {
            return user.Id == HostId && Users.Count == MaxUsers;
        }

        public void Serialize(NetDataWriter writer) {
            writer.PutArray(Users.ToArray());
            writer.Put(HostId);

            writer.Put(UpdateType.ToString());
        }

        public void Deserialize(NetDataReader reader) {
            Users = reader.GetArray<LobbyUser.Data>().ToList();
            HostId = reader.GetString();

            Enum.TryParse(reader.GetString(), out UpdateType);
        }

        public enum EUpdateType { }
    }
}
