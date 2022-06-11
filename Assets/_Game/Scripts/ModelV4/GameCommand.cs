using System;
using _Game.Scripts.Utils;
using GeneralUtils.Command;

namespace _Game.Scripts.ModelV4 {
    public abstract class GameCommand : Command, ISerializable {
        private GameDataAPI _api;

        public void ProvideDataApi(GameDataAPI api) {
            _api = api;
        }

        protected override void PerformDo() {
            PerformDoOnData(_api);
        }

        protected abstract void PerformDoOnData(GameDataAPI api);

        public abstract string SerializeContents();
        public abstract void DeserializeContents(string contents);
    }
}
