using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game {
    public static class CommandBuilder {
        public static bool TryBuildMoveCommand(FieldAccessor accessor, IReadOnlyEntity entity, Vector2Int destination,
            [CanBeNull] out GameCommand command, out Vector2Int[] path) {
            var positionComponent = entity.GetReadOnlyComponent<PositionData>();
            var moveComponent = entity.GetReadOnlyComponent<MoveData>();
            if (positionComponent == null || moveComponent == null) {
                command = null;
                path = null;
                return false;
            }
            
            var calculatedPath = accessor.CalculatePath(entity, positionComponent.Data.Position, destination);
            if (calculatedPath == null) {
                command = null;
                path = null;
                return false;
            }

            command = calculatedPath.Length - 1 > moveComponent.Data.RemainingDistance
                ? null
                : new MoveCommand {
                    EntityId = entity.Id, 
                    Position = destination
                };
            path = calculatedPath;
            return true;
        }

        public static bool TryBuildAttackCommand(FieldAccessor accessor, IReadOnlyEntity entity, Vector2Int destination,
            [CanBeNull] out GameCommand command, out IReadOnlyEntity targetEntity) {
            var positionComponent = entity.GetReadOnlyComponent<PositionData>();
            var attackComponent = entity.GetReadOnlyComponent<AttackData>();

            var possibleTarget = accessor.TryGetEntitiesAt(destination).FirstOrDefault();
            var healthComponent = possibleTarget?.GetReadOnlyComponent<HealthData>();
            if (positionComponent == null || attackComponent == null || healthComponent == null) {
                command = null;
                targetEntity = null;
                return false;
            }

            if (!Attacking.CanAttack(accessor, entity, possibleTarget)) {
                command = null;
                targetEntity = null;
                return false;
            }
            
            command = !attackComponent.Data.CanAttack
                ? null
                : new AttackCommand {
                    EntityId = entity.Id, 
                    TargetId = possibleTarget.Id
                };
            targetEntity = possibleTarget;
            return true;
        }
    }
}