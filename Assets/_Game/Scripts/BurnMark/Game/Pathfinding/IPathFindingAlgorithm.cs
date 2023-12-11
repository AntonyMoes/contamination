using _Game.Scripts.BurnMark.Game.Presentation;
using JetBrains.Annotations;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Pathfinding {
    public interface IPathFindingAlgorithm {
        public void SetAccessor(FieldAccessor fieldAccessor);
        [CanBeNull]
        public Vector2Int[] CalculatePath(Vector2Int from, Vector2Int to);
    }
}