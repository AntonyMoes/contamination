using System;
using GeneralUtils.Command;
using LiteNetLib.Utils;

namespace _Game.Scripts.NetworkModel.Commands {
    public abstract class GameCommand : Command {
        private const string TypeSeparator = "|";

        private IGameAPI _api;

        public void ProvideDataApi(IGameAPI api) {
            _api = api;
        }

        protected sealed override void PerformDo() {
            PerformDoOnData(_api);
        }

        protected abstract void PerformDoOnData(IGameAPI api);

        public void Serialize(NetDataWriter writer) {
            writer.Put(GetType().AssemblyQualifiedName);
            PerformSerializeRest(writer);
        }

        public static GameCommand Deserialize(NetDataReader reader) {
            var typeName = reader.GetString();
            var type = Type.GetType(typeName);
            var command = (GameCommand) Activator.CreateInstance(type);

            command.PerformDeserializeRest(reader);
            return command;
        }

        protected abstract void PerformSerializeRest(NetDataWriter writer);
        protected abstract void PerformDeserializeRest(NetDataReader reader);

        public sealed override string ToString() {
            return $"{GetType().AssemblyQualifiedName}{TypeSeparator}{ContentsToString()}";
        }

        protected virtual string ContentsToString() => string.Empty;
    }
}
