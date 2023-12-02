using System;
using System.Linq;
using _Game.Scripts.ModelV4;
using _Game.Scripts.TicTacToe.Game.Data;

namespace _Game.Scripts.TicTacToe.Game.Commands {
    public class MarkCommand : TicTacToeCommand {
        public MarkData.EMark Mark;
        public int EntityId;

        protected override void PerformDoOnAPI(GameDataAPI api) {
            var entityToMark = api.ModifiableEntities.First(entity => entity.Id == EntityId);
            entityToMark.GetModifiableComponent<MarkData>().Data = new MarkData {
                Mark = Mark
            };
            api.EndTurn();
        }

        protected override string SerializeContents() {
            return $"{Mark};{EntityId}";
        }

        protected override void DeserializeContents(string contents) {
            var args = contents.Split(';');
            Enum.TryParse(args[0], out Mark);
            EntityId = int.Parse(args[1]);
        }
    }
}
