using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct FieldObjectData : ISame<FieldObjectData> {
        public FieldObjectConfig Config;

        public bool IsSame(FieldObjectData other) {
            return Config == other.Config;
        }
    }
}