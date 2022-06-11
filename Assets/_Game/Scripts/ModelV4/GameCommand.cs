using System;
using _Game.Scripts.Utils;
using GeneralUtils.Command;

namespace _Game.Scripts.ModelV4 {
    public abstract class GameCommand : Command {
        private GameDataAPI _api;

        public void ProvideDataApi(GameDataAPI api) {
            _api = api;
        }

        protected override void PerformDo() {
            PerformDoOnData(_api);
        }

        protected abstract void PerformDoOnData(GameDataAPI api);

        protected abstract string SerializeContents();
        protected abstract void DeserializeContents(string contents);

        private const string TypeSeparator = "|";

        public string Serialize() {
            return $"{GetType().AssemblyQualifiedName}{TypeSeparator}{SerializeContents()}";
        }

        public static GameCommand Deserialize(string serializedCommand) {
            var separatorPosition = serializedCommand.IndexOf(TypeSeparator, StringComparison.Ordinal);

            var typeName = serializedCommand.Substring(0, separatorPosition);
            var type = Type.GetType(typeName);
            var command = (GameCommand) Activator.CreateInstance(type);

            var serializedContents = serializedCommand.Length > separatorPosition + TypeSeparator.Length
                ? serializedCommand.Substring(separatorPosition + TypeSeparator.Length)
                : "";
            command.DeserializeContents(serializedContents);
            return command;
        }
    }
}
