using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Pathfinding {
    public interface IPathFindingAlgorithm {
        public void SetAccessor(FieldAccessor fieldAccessor);
        [CanBeNull]
        public (Vector2Int, int)[] CalculatePath(IReadOnlyEntity entity, Vector2Int from, Vector2Int to);
    }
}