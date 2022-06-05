using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public abstract class Message : INetSerializable {
        public abstract void Serialize(NetDataWriter writer);
        public abstract void Deserialize(NetDataReader reader);
    }

    public abstract class EmptyMessage : Message {
        public sealed override void Serialize(NetDataWriter writer) { }

        public sealed override void Deserialize(NetDataReader reader) { }
    }
}
