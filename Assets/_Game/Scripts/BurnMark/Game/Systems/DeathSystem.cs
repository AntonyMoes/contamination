using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Systems {
    public class DeathSystem : ModelV4.ECS.Systems.UpdateEntitySystem {
        protected override IEnumerable<IComponent> GetComponents(IEntity entity) {
            return new IComponent[] {
                entity.GetReadOnlyComponent<HealthData>()
            };
        }

        protected override void OnEntityUpdate(IEntity entity) {
            var healthData = entity.GetReadOnlyComponent<HealthData>()!;
            if (healthData.Data.Health <= 0) {
                GameAPI.RemoveEntity(entity.Id);
            }
        }
    }
}