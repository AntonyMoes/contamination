using _Game.Scripts.BurnMark.Game.Data.Configs;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct UnitData : ISame<UnitData> {
        public UnitConfig Config;

        public bool IsSame(UnitData other) {
            return Config == other.Config;
        }
    }
}