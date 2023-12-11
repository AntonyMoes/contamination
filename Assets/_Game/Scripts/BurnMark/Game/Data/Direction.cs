using System;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data {
    public enum Direction {
        Right = 1,
        DownRight = 2,
        DownLeft = 3,
        Left = 4,
        UpLeft = 5,
        UpRight = 6
    }

    public static class DirectionUtils {
        public static readonly Direction[] Directions = new [] {
            Direction.Right,
            Direction.DownRight,
            Direction.DownLeft,
            Direction.Left,
            Direction.UpLeft,
            Direction.UpRight
        };

        public static Vector2Int ToVector2Int(this Direction direction, Vector2Int position) {
            return direction switch {
                Direction.Right => Vector2Int.right,
                Direction.DownRight => position.y % 2 == 0 ? Vector2Int.down : new Vector2Int(1, -1),
                Direction.DownLeft => position.y % 2 == 0 ? -Vector2Int.one : Vector2Int.down,
                Direction.Left => Vector2Int.left,
                Direction.UpLeft => position.y % 2 == 0 ? new Vector2Int(-1, 1) : Vector2Int.up,
                Direction.UpRight => position.y % 2 == 0 ? Vector2Int.up : Vector2Int.one,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}