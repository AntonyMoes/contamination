using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Commands {
    public abstract class StringSerializedGameCommand : GameCommand {
        protected sealed override void PerformSerializeRest(NetDataWriter writer) {
            writer.Put(SerializeContents());
        }

        protected sealed override void PerformDeserializeRest(NetDataReader reader) {
            DeserializeContents(reader.GetString());
        }

        protected abstract string SerializeContents();
        protected abstract void DeserializeContents(string contents);

        protected sealed override string ContentsToString() {
            return SerializeContents();
        }
    }
}