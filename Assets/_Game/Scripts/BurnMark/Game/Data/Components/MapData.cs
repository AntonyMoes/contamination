using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct MapData : ISame<MapData> {
        public Vector2Int Size;

        public bool IsSame(MapData other) {
            return Size == other.Size;
        }
    }
}