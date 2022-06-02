using System;
using GeneralUtils.Command;

namespace _Game.Scripts.ModelV4 {
    public abstract class GameCommand : ReversibleCommand {
        private GameDataAPI _api;

        public void ProvideDataApi(GameDataAPI api) {
            _api = api;
        }

        protected override Action PerformReversibleDo() {
            var undo = PerformReversibleDoOnData(_api);
            return () => undo(_api);
        }

        protected abstract Action<GameDataAPI> PerformReversibleDoOnData(GameDataAPI api);
    }
}
