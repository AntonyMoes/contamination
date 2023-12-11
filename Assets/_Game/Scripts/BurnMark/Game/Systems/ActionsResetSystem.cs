using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.ECS.Systems;

namespace _Game.Scripts.BurnMark.Game.Systems {
    public class ActionsResetSystem : ModelV4.ECS.Systems.System, IStartTurnSystem {
        protected override IEnumerable<IComponent> GetComponents(IEntity entity) {
            return new IComponent[] {
                entity.GetModifiableComponent<MoveData>(),
                entity.GetModifiableComponent<OwnerData>()
            };
        }

        public void OnStartTurn(int user, int turn) {
            foreach (var entity in TrackedEntities) {
                var moveComponent = entity.GetModifiableComponent<MoveData>()!;
                var ownerComponent = entity.GetModifiableComponent<OwnerData>()!;

                if (!(ownerComponent.Data.Owner is { } ownerId) || ownerId != user) {
                    continue;
                }

                moveComponent.Data = moveComponent.Data.ResetMovement();
            }
        }
    }
}