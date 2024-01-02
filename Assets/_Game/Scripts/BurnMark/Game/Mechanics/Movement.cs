using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Mechanics {
    public static class Movement {
        public static bool CanMoveThrough(FieldAccessor accessor, IReadOnlyEntity entity, Vector2Int from, Vector2Int position) {
            if (!accessor.Terrain.TryGetValue(position, out var terrain)) {
                return false;
            }

            if (Mathf.Abs(accessor.Terrain[from].Data.Height - terrain.Data.Height) > 1) {
                return false;
            }

            var owner = entity.GetOwnerId();
            return accessor.TryGetEntitiesAt(position)
                .Select(e => e.GetOwnerId())
                .All(o => o == owner);
        }

        public static bool CanSpawnOn(FieldAccessor accessor, Vector2Int position) {
            return CanFinishOn(accessor, null, position);  // TODO forbid spawning on uneven heights?
        }

        public static bool CanFinishOn(FieldAccessor accessor, IReadOnlyEntity entity, Vector2Int position) {
            return !accessor.Units.ContainsKey(position)
                   && !accessor.FieldObjects.ContainsKey(position)
                   && accessor.Terrain.ContainsKey(position);
        }

        public static IEnumerable<Vector2Int> GetAdjacent(Vector2Int position) {
            return DirectionUtils.Directions
                .Select(dir => dir.ToVector2Int(position))
                .Select(offset => position + offset);
        }
        
        public static int StepsOfPathCanTraverse(MoveData moveData, (Vector2Int, int)[] path) {
            var remainingMovement = moveData.RemainingDistance;
            if (remainingMovement <= 0) {
                return 0;
            }

            for (var i = 0; i < path.Length; i++) {
                remainingMovement -= path[i].Item2;
                if (remainingMovement <= 0) {
                    return i + 1;
                }
            }

            return path.Length;
        }
    }
}