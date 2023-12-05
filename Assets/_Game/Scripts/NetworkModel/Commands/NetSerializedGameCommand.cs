using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Commands {
    public abstract class NetSerializedGameCommand : GameCommand{
        protected sealed override void PerformSerializeRest(NetDataWriter writer) {
            SerializeContents(writer);
        }

        protected sealed override void PerformDeserializeRest(NetDataReader reader) {
            DeserializeContents(reader);
        }

        protected abstract void SerializeContents(NetDataWriter writer);
        protected abstract void DeserializeContents(NetDataReader reader);
    }
}