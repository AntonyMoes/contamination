using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class RoomUpdateMessage<TRoomSettings> : Message where TRoomSettings : IRoomSettings<TRoomSettings>, new() {
        public string RoomId;
        public bool UpdatePassword;
        public string Password;
        public bool UpdateSettings;
        public TRoomSettings Settings;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(RoomId);
            writer.Put(UpdatePassword);
            writer.Put(Password);
            writer.Put(UpdateSettings);
            writer.Put(Settings);
        }

        public override void DeserializeContents(NetDataReader reader) {
            RoomId = reader.GetString();
            UpdatePassword = reader.GetBool();
            Password = reader.GetString();
            UpdateSettings = reader.GetBool();
            Settings = reader.Get<TRoomSettings>();
        }
    }
}
