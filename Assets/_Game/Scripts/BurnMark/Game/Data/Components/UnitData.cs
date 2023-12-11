using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct UnitData : ISame<UnitData> {
        public bool IsSame(UnitData other) {
            return true;
        }
    }
}