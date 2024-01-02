using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Configs.Terrain;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    [Serializable]
    public struct TerrainData : ISame<TerrainData> {
        public int Height;
        public TerrainFeatureConfig[] Features;

        public int MoveDifficulty => 1 + Features.Sum(f => f.AdditionalMovementCost);

        public bool IsSame(TerrainData other) {
            return Height == other.Height
                   && Features.ListEquals(other.Features);
        }
    }
}