using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Mechanics {
    public static class Position {
        public static Vector2 Map(Vector2Int position, float tileSize) {
            var xSize = Mathf.Sqrt(3 * tileSize);
            var xOffset = position.y % 2 == 0 ? 0f : xSize / 2f;
            var yOffset = tileSize * 1.5f;
            return new Vector2(position.x * xSize + xOffset, position.y * yOffset);
        }

        public static Vector2 GetFieldCenter(Vector2Int fieldSize, float tileSize) {
            // var xSize = Mathf.Sqrt(3 * tileSize);
            // var xOffset = fieldSize.x > 1
            //     ? xSize / 4f
            //     : 0f;
            // var yOffset = tileSize * 1.5f;
            // return new Vector2((fieldSize.x - 1) * xSize / 2f + xOffset, (fieldSize.y - 1) * yOffset/ 2f);
            return GetFieldMax(fieldSize, tileSize) / 2f;
        }

        public static Vector2 GetFieldMax(Vector2Int fieldSize, float tileSize) {
            var xSize = Mathf.Sqrt(3 * tileSize);
            var xOffset = fieldSize.x > 1
                ? xSize / 2f
                : 0f;
            var yOffset = tileSize * 1.5f;
            return new Vector2((fieldSize.x - 1) * xSize + xOffset, (fieldSize.y - 1) * yOffset);
        }

        public static int Distance(Vector2Int from, Vector2Int to) {
            var delta = to - from;
            var evenY = from.y % 2 == 0;
            var negativeX = delta.x < 0;
            
            var deltaX = Mathf.Abs(delta.x);
            var deltaY = Mathf.Abs(delta.y);

            var xCorrection = evenY == negativeX
                ? (deltaY + 1) / 2
                : deltaY / 2;

            return Mathf.Max(deltaX - xCorrection, 0) + deltaY;
        }
    }
}