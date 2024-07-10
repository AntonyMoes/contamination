using System;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    [Serializable]
    public struct ResourceGainData : ISame<ResourceGainData> {
        public Resources Resources;

        public bool IsSame(ResourceGainData other) {
            return Resources.IsSame(other.Resources);
        }
    }
}