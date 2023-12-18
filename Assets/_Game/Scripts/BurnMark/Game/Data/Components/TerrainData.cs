using System;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    [Serializable]
    public struct TerrainData : ISame<TerrainData> {
        public int Height;

        public bool IsSame(TerrainData other) {
            return Height == other.Height;
        }
    }
}