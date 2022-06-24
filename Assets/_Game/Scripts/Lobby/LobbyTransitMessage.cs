using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public class LobbyTransitMessage : Message {
        public bool Join;
        public string Name;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(Join);
            writer.Put(Name);
        }

        public override void DeserializeContents(NetDataReader reader) {
            Join = reader.GetBool();
            Name = reader.GetString();
        }
    }
}
