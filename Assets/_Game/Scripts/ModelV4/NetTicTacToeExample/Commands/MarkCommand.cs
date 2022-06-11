using System;
using System.Linq;
using _Game.Scripts.ModelV4.NetTicTacToeExample.Data;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample.Commands {
    public class MarkCommand : GameCommand {
        public MarkData.EMark Mark;
        public int EntityId;

        protected override void PerformDoOnData(GameDataAPI api) {
            var entityToMark = api.Entities.First(entity => entity.Id == EntityId);
            entityToMark.GetModifiableComponent<MarkData>().Data = new MarkData {
                Mark = Mark
            };
            api.EndTurn();
        }

        public override string SerializeContents() {
            return $"{Mark};{EntityId}";
        }

        public override void DeserializeContents(string contents) {
            var args = contents.Split(';');
            Enum.TryParse(args[0], out Mark);
            EntityId = int.Parse(args[1]);
        }
    }
}
