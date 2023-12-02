using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Lobby;
using _Game.Scripts.TicTacToe.Game.Data;
using LiteNetLib.Utils;

namespace _Game.Scripts.TicTacToe.Network {
    public class TicTacToeRoomSettings : IRoomSettings<TicTacToeRoomSettings> {
        public const int MaxUsers = 2;

        public List<LobbyUser.Data> Users = new List<LobbyUser.Data>();
        public string HostId;
        public MarkData.EMark[] Marks = {MarkData.EMark.X, MarkData.EMark.O};

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

        public bool CanMakeUpdate(LobbyUser.Data user, TicTacToeRoomSettings update) {
            return user.Id == HostId;
        }

        public void MakeUpdate(TicTacToeRoomSettings update) {
            switch (update.UpdateType) {
                case EUpdateType.SwitchMarks:
                    Marks = Marks.Reverse().ToArray();
                    break;
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
            writer.PutEnumArray(Marks);

            writer.Put(UpdateType.ToString());
        }

        public void Deserialize(NetDataReader reader) {
            Users = reader.GetArray<LobbyUser.Data>().ToList();
            HostId = reader.GetString();
            Marks = reader.GetEnumArray<MarkData.EMark>();

            Enum.TryParse(reader.GetString(), out UpdateType);
        }

        public enum EUpdateType {
            SwitchMarks
        }
    }
}
