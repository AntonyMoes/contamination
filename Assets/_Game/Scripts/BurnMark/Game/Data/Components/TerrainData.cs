using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct TerrainData : ISame<TerrainData> {
        public bool IsSame(TerrainData other) {
            return true;
        }
    }
}