using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Field : MonoBehaviour {
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private GameObject _tilePrefab;
        [SerializeField] private float _tileSize;
        [SerializeField] private GameObject _cameraContainer;

        private float XSize => Mathf.Sqrt(3 * _tileSize);
        private float ZOffset => _tileSize * 1.5f;

        private readonly List<GameObject> _tiles = new List<GameObject>();

        public void Initialize(Vector2Int fieldSize) {
            for (var i = 0; i < fieldSize.x; i++) {
                for (var j = 0; j < fieldSize.y; j++) {
                    var tile = Instantiate(_tilePrefab, _tilesParent);
                    var xOffset = j % 2 == 0 ? 0f : XSize / 2f;
                    var position = new Vector3(i * XSize + xOffset, 0f, j * ZOffset);
                    tile.transform.localPosition = position;
                    _tiles.Add(tile);
                }
            }

            _cameraContainer.transform.position = GetFieldCenter(fieldSize);
        }

        private Vector3 GetFieldCenter(Vector2Int fieldSize) {
            var xOffset = fieldSize.x > 1
                ? XSize / 4f
                : 0f;
            return new Vector3((fieldSize.x - 1) * XSize / 2f + xOffset, 0f, (fieldSize.y - 1) * ZOffset/ 2f);
        }

        public void Clear() {
            foreach (var tile in _tiles) {
                Destroy(tile);
            }
            _tiles.Clear();
        }
    }
}