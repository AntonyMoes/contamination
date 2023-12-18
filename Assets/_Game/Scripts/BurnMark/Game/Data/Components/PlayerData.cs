using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct PlayerData : ISame<PlayerData> {
        public Color Color;

        public bool IsSame(PlayerData other) {
            return Color == other.Color;
        }
    }
}