using System.Linq;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions {
    public static class FieldActions {
        [CanBeNull]
        public static IReadOnlyEntity TrySelectTile(FieldAccessor accessor, Vector2Int position) {
            return accessor.Units.TryGetValue(position, out var selectedUnit)
                ? selectedUnit
                : accessor.FieldObjects.TryGetValue(position, out var fieldObject)
                    ? fieldObject
                    : null;
        }

        [CanBeNull]
        public static IFieldAction TryGetAction(FieldAccessor accessor, [CanBeNull] IReadOnlyEntity selectedEntity, Vector2Int targetPosition) {
            if (selectedEntity == null) {
                return null;
            }

            if (selectedEntity.GetReadOnlyComponent<PositionData>()!.Data.Position == targetPosition && selectedEntity.GetReadOnlyComponent<MoveData>() is {} moveComponent) {
                var moveDistance = moveComponent.Data.Distance;
                var tiles = accessor.Terrain.Keys
                    .Where(pos => {
                        var distance = Position.Distance(targetPosition, pos);
                        return distance >= 1 && distance <= moveDistance;
                    })
                    .ToArray();
                return new DebugShowMoveRangeAction(tiles);
            }

            if (CommandBuilder.TryBuildMoveCommand(accessor, selectedEntity, targetPosition, out var moveCommand, out var path)) {
                return new MoveFieldAction(moveCommand, selectedEntity, path);
            }

            if (CommandBuilder.TryBuildAttackCommand(accessor, selectedEntity, targetPosition, out var attackCommand, out var targetEntity)) {
                return new AttackFieldAction(attackCommand, selectedEntity, targetEntity);
            }

            return null;
        }
    }
}