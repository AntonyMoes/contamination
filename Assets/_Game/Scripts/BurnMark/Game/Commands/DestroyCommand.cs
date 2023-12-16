using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class DestroyCommand : GameCommand {
        public int EntityId;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.Put(EntityId);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            EntityId = reader.GetInt();
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            api.RemoveEntity(EntityId);
        }
    }
}