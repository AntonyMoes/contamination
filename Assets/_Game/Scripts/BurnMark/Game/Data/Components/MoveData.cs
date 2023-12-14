using System;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    [Serializable]
    public struct MoveData : ISame<MoveData> {
        public int Distance;
        [HideInInspector] public bool CanMove;

        public int RemainingDistance => CanMove ? Distance : 0;

        public bool IsSame(MoveData other) {
            return Distance == other.Distance
                && CanMove == other.CanMove;
        }

        public MoveData ResetMovement() {
            return new MoveData {
                Distance = Distance,
                CanMove = true
            };
        }

        public MoveData Move() {
            return new MoveData {
                Distance = Distance,
                CanMove = false
            };
        }
    }
}