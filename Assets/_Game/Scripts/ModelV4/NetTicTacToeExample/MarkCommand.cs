using System;
using System.Linq;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class MarkCommand : GameCommand {
        public MarkComponent.EMark Mark;
        public int EntityId;

        protected override Action<GameDataAPI> PerformReversibleDoOnData(GameDataAPI api) {
            var entityToMark = api.Entities.First(entity => entity.Id == EntityId);
            var markComponent = entityToMark.GetModifiableComponent<MarkComponent, MarkComponent.MarkData>();
            var oldMark = markComponent.Data.Mark;
            markComponent.Data = new MarkComponent.MarkData {
                Mark = Mark
            };
            api.EndTurn();

            return api => {
                api.UndoEndTurn();
                var entityToUnmark = api.Entities.First(entity => entity.Id == EntityId);
                var markComponent = entityToUnmark.GetModifiableComponent<MarkComponent, MarkComponent.MarkData>();
                markComponent.Data = new MarkComponent.MarkData {
                    Mark = oldMark
                };
            };
        }

        public override string SerializeContents() {
            return $"{Mark};{EntityId};";
        }

        public override void DeserializeContents(string contents) {
            var args = contents.Split(';');
            Enum.TryParse(args[0], out Mark);
            EntityId = int.Parse(args[1]);
        }
    }
}
