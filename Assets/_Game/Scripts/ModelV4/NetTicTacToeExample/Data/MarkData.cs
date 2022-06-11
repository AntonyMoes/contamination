﻿using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample.Data {
    public struct MarkData : ISame<MarkData> {
        public EMark Mark;

        public bool IsSame(ISame<MarkData> other) {
            return Mark == other.Get().Mark;
        }

        public enum EMark {
            None,
            X,
            O
        }
    }
}
