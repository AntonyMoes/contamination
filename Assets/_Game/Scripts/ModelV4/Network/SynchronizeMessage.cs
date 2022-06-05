using _Game.Scripts.Network;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.Network {
    public class SynchronizeMessage : Message {
        public string Guid;

        public override void Serialize(NetDataWriter writer) {
            writer.Put(Guid);
        }

        public override void Deserialize(NetDataReader reader) {
            Guid = reader.GetString();
        }
    }
}
