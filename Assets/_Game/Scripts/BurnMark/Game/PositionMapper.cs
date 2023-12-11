using UnityEngine;

namespace _Game.Scripts.BurnMark.Game {
    public static class PositionMapper {
        public static Vector2 Map(Vector2Int position, float tileSize) {
            var xSize = Mathf.Sqrt(3 * tileSize);
            var xOffset = position.y % 2 == 0 ? 0f : xSize / 2f;
            var yOffset = tileSize * 1.5f;
            return new Vector2(position.x * xSize + xOffset, position.y * yOffset);
        }

        public static Vector2 GetFieldCenter(Vector2Int fieldSize, float tileSize) {
            var xSize = Mathf.Sqrt(3 * tileSize);
            var xOffset = fieldSize.x > 1
                ? xSize / 4f
                : 0f;
            var yOffset = tileSize * 1.5f;
            return new Vector3((fieldSize.x - 1) * xSize / 2f + xOffset, 0f, (fieldSize.y - 1) * yOffset/ 2f);
        }
    }
}