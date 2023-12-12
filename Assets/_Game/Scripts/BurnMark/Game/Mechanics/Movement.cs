using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Mechanics {
    public static class Movement {
        public static bool CanMoveThrough(FieldAccessor accessor, IReadOnlyEntity entity, Vector2Int position) {
            return accessor.Terrain.ContainsKey(position);
        }

        public static bool CanFinishOn(FieldAccessor accessor, IReadOnlyEntity entity, Vector2Int position) {
            return !accessor.TryGetUnitAt(position, out _)
                   && !accessor.TryGetFieldObjectAt(position, out _)
                   && accessor.Terrain.ContainsKey(position);
        }
    }
}