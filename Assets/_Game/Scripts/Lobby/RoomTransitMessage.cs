using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class RoomTransitMessage : Message {
        public string RoomId;
        public bool Join;
        public string Password;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(RoomId);
            writer.Put(Join);
            writer.Put(Password);
        }

        public override void DeserializeContents(NetDataReader reader) {
            RoomId = reader.GetString();
            Join = reader.GetBool();
            Password = reader.GetString();
        }
    }
}
