using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.ECS.Systems;

namespace _Game.Scripts.BurnMark.Game.Systems {
    public class ResourceGainSystem : ModelV4.ECS.Systems.System, IStartTurnSystem {
        protected override IEnumerable<IComponent> GetComponents(IEntity entity) {
            return new IComponent[] {
                entity.GetModifiableComponent<ResourceGainData>(), 
                entity.GetModifiableComponent<OwnerData>()
            };
        }

        public void OnStartTurn(int user, int turn) {
            if (turn == 0) {
                return;
            }

            foreach (var entity in TrackedEntities) {
                var resourceGainComponent = entity.GetModifiableComponent<ResourceGainData>()!;
                var ownerComponent = entity.GetModifiableComponent<OwnerData>()!;

                if (!(ownerComponent.Data.Owner is { } ownerId) || ownerId != user) {
                    continue;
                }

                var owner = GetOwner(ownerId);
                var resourceComponent = owner.Entity.GetModifiableComponent<ResourceData>()!;
                resourceComponent.Data = resourceComponent.Data.Gain(resourceGainComponent.Data);
            }
        }

        private IComponent<ResourceData> GetOwner(int owner) {
            return GameAPI.ModifiableEntities
                .GetModifiableComponent<ResourceData>()
                .First(component => component.Entity.GetOwnerId() == owner);
        }
    }
}