using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class BuildUnitCommand : GameCommand {
        public int BuilderId;
        public int UnitConfigOrder;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.Put(BuilderId);
            writer.Put(UnitConfigOrder);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            BuilderId = reader.GetInt();
            UnitConfigOrder = reader.GetInt();
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            var builder = api.ModifiableEntities.Values
                .GetModifiableComponent<UnitBuilderData>()
                .First(c => c.Entity.Id == BuilderId);

            var config = builder.Data.BuildableUnits[UnitConfigOrder];
            var owner = builder.Entity.GetInOwner<ResourceData>(api)!;
            owner.Data.TryPay(config.Cost, out var newResources);
            owner.Data = newResources;

            builder.Data = builder.Data.BuildUnit(UnitConfigOrder);
        }
    }
}