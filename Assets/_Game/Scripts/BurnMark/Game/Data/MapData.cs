using System;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data {
    [Serializable]
    public class MapData {
        public Vector2Int MapSize;
        public Vector2Int[] PlayerBases;
    }
}