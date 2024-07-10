using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Systems {
    public class CaptureSystem : ModelV4.ECS.Systems.UpdateEntitySystem {
        private readonly FieldAccessor _accessor;

        public CaptureSystem(FieldAccessor fieldAccessor) {
            _accessor = fieldAccessor;
        }

        protected override IEnumerable<IComponent> GetComponents(IEntity entity) {
            return new IComponent[] {
                entity.GetReadOnlyComponent<OwnerData>(),
                entity.GetReadOnlyComponent<PositionData>(),
                entity.GetReadOnlyComponent<MoveData>(),
            };
        }

        protected override void OnEntityUpdate(IEntity entity) {
            var maybeOwner = entity.GetReadOnlyComponent<OwnerData>()!.Data.Owner;
            if (!(maybeOwner is { } owner)) {
                return;
            }

            var position = entity.GetReadOnlyComponent<PositionData>()!.Data.Position;
            var capturedEntity = GameAPI.ModifiableEntities[_accessor.Terrain[position].ReadOnlyEntity.Id];
            var capturableComponent = capturedEntity.GetModifiableComponent<CapturableData>();
            var ownerComponent = capturedEntity.GetModifiableComponent<OwnerData>();
            if (capturableComponent == null || ownerComponent == null || !capturableComponent.Data.CanBeCaptured) {
                return;
            }

            ownerComponent.Data = ownerComponent.Data.WithOwner(owner);
        }
    }
}