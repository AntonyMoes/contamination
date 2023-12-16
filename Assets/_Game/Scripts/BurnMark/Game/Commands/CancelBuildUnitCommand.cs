using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class CancelBuildUnitCommand : GameCommand {
        public int BuilderId;
        public int QueuePosition;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.Put(BuilderId);
            writer.Put(QueuePosition);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            BuilderId = reader.GetInt();
            QueuePosition = reader.GetInt();
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            var builder = api.ModifiableEntities.Values
                .GetModifiableComponent<UnitBuilderData>()
                .First(c => c.Entity.Id == BuilderId);

            var config = QueuePosition == UnitBuilderData.CurrentPosition
                ? builder.Data.Current!
                : builder.Data.Queue[QueuePosition];
            var owner = builder.Entity.GetInOwner<ResourceData>(api)!;
            owner.Data.TryPay(config.Cost.Refund(), out var newResources);
            owner.Data = newResources;

            builder.Data = builder.Data.CancelBuilding(QueuePosition);
        }
    }
}