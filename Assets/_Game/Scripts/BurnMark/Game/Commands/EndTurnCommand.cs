using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class EndTurnCommand : GameCommand {
        protected override void SerializeContents(NetDataWriter writer) { }

        protected override void DeserializeContents(NetDataReader reader) { }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            api.EndTurn();
        }
    }
}