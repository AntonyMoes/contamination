using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct PositionData : ISame<PositionData> {
        public Vector2Int Position; 

        public bool IsSame(PositionData other) {
            return Position == other.Position;
        }
    }
}