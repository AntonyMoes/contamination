using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class AttackCommand : GameCommand {
        public int EntityId;
        public int TargetId;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.Put(EntityId);
            writer.Put(TargetId);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            EntityId = reader.GetInt();
            TargetId = reader.GetInt();
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            var entity = api.GetModifiableEntity(EntityId)!;
            var attackComponent = entity.GetModifiableComponent<AttackData>()!;

            var target = api.GetModifiableEntity(TargetId)!;
            var healthComponent = target.GetModifiableComponent<HealthData>()!;

            var damage = Attacking.CalculateDamage(attackComponent.Data, healthComponent.Data);
            attackComponent.Data = attackComponent.Data.Attack();
            healthComponent.Data = healthComponent.Data.TakeDamage(damage);
        }
    }
}