using _Game.Scripts.Utils;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public abstract class Message : INetSerializable {
        public string Id { get; private set; } = Guid.New;
        public string RespondsToId { get; private set; }

        public void SetRespondsToId(string id) {
            RespondsToId = id;
        }

        public void Serialize(NetDataWriter writer) {
            writer.Put(Id);
            writer.Put(RespondsToId);
            SerializeContents(writer);
        }

        public void Deserialize(NetDataReader reader) {
            Id = reader.GetString();
            RespondsToId = reader.GetString();
            DeserializeContents(reader);
        }

        public abstract void SerializeContents(NetDataWriter writer);
        public abstract void DeserializeContents(NetDataReader reader);
    }

    public abstract class EmptyMessage : Message {
        public sealed override void SerializeContents(NetDataWriter writer) { }

        public sealed override void DeserializeContents(NetDataReader reader) { }
    }
}
