using System;
using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class LobbyUpdateMessage<TRoomSettings> : Message where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        public EActionType ActionType;
        public EContentType ContentType;
        public LobbyUser.Data User;
        public Room<TRoomSettings>.Data Room;

        public enum EActionType {
            CreateOrUpdate,
            Delete
        }

        public enum EContentType {
            User,
            Room
        }

        public void Handle(Action<LobbyUser.Data, EActionType> userHandler, Action<Room<TRoomSettings>.Data, EActionType> roomHandler) {
            switch (ContentType) {
                case EContentType.User:
                    userHandler?.Invoke(User, ActionType);
                    break;
                case EContentType.Room:
                    roomHandler?.Invoke(Room, ActionType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(ActionType.ToString());
            writer.Put(ContentType.ToString());
            writer.PutNullable(User);
            writer.PutNullable(Room);
        }

        public override void DeserializeContents(NetDataReader reader) {
            Enum.TryParse(reader.GetString(), out ActionType);
            Enum.TryParse(reader.GetString(), out ContentType);
            User = reader.GetNullable<LobbyUser.Data>();
            Room = reader.GetNullable<Room<TRoomSettings>.Data>();
        }
    }
}
