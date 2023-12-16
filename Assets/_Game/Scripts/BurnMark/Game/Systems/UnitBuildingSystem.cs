using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.ECS.Systems;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Systems {
    public class UnitBuildingSystem : ModelV4.ECS.Systems.System, IStartTurnSystem{
        private readonly FieldAccessor _accessor;

        public UnitBuildingSystem(FieldAccessor fieldAccessor) {
            _accessor = fieldAccessor;
        }

        protected override IEnumerable<IComponent> GetComponents(IEntity entity) {
            return new IComponent[] {
                entity.GetModifiableComponent<OwnerData>(),
                entity.GetModifiableComponent<UnitBuilderData>(),
                entity.GetModifiableComponent<PositionData>()
            };
        }

        public void OnStartTurn(int user, int turn) {
            foreach (var entity in TrackedEntities) {
                var owner = entity.GetModifiableComponent<OwnerData>()!;
                if (owner.Data.Owner != user) {
                    continue;
                }

                var builder = entity.GetModifiableComponent<UnitBuilderData>()!;
                var position = entity.GetModifiableComponent<PositionData>()!;

                var afterWorkData = builder.Data.PerformWork(out var unitReadyToBeBuilt);
                if (unitReadyToBeBuilt == null) {
                    builder.Data = afterWorkData;
                    continue;
                }

                var maybeSpawnPosition = GetSpawnPosition(position.Data.Position);
                if (!(maybeSpawnPosition is { } spawnPosition)) {
                    builder.Data = afterWorkData;
                    continue;
                }

                builder.Data = afterWorkData.FinishBuilding(out var builtUnit);
                GameAPI.AddEntity(builtUnit!.Create(user, spawnPosition));
            }
        }

        private Vector2Int? GetSpawnPosition(Vector2Int builderPosition) {
            return Movement.GetAdjacent(builderPosition)
                .Where(pos => Movement.CanSpawnOn(_accessor, pos))
                .Select(pos => (Vector2Int?) pos)
                .FirstOrDefault();
        }
    }
}