using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Network {
    public class SynchronizeMessage : Message {
        public string Guid;

        public override void SerializeContents(NetDataWriter writer) {
            writer.Put(Guid);
        }

        public override void DeserializeContents(NetDataReader reader) {
            Guid = reader.GetString();
        }
    }
}
