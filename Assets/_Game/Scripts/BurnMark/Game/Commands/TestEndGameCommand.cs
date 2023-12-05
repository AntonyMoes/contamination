using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class TestEndGameCommand : GameCommand {
        public int Winner;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.Put(Winner);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            Winner = reader.GetInt();
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            api.EndTurn(true);
        }
    }
}