using System;
using _Game.Scripts.Utils;
using GeneralUtils.Command;

namespace _Game.Scripts.ModelV4 {
    public abstract class GameCommand : ReversibleCommand, ISerializable {
        private GameDataAPI _api;

        public void ProvideDataApi(GameDataAPI api) {
            _api = api;
        }

        protected override Action PerformReversibleDo() {
            var undo = PerformReversibleDoOnData(_api);
            return () => undo(_api);
        }

        protected abstract Action<GameDataAPI> PerformReversibleDoOnData(GameDataAPI api);

        public abstract string SerializeContents();
        public abstract void DeserializeContents(string contents);
    }
}
