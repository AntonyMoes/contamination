using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Mechanics;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Field : MonoBehaviour {
        [Header("Tiles")]
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private float _tileSize;

        [Header("Objects")]
        [SerializeField] private Transform _objectsParent;
        [SerializeField] private GameObject _basePrefab;
        [SerializeField] private GameObject _carPrefab;

        [Header("RandomTest")]
        [FormerlySerializedAs("_tileWeights")]
        [SerializeField] private float[] _heightWeights;
        [SerializeField] private int[] _heights;

        [Header("Camera")]
        [SerializeField] private GameObject _cameraContainer;

        private readonly Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
        private readonly Dictionary<Tile, Vector2Int> _tilePositions = new Dictionary<Tile, Vector2Int>();
        private readonly Dictionary<Vector2Int, GameObject> _objects = new Dictionary<Vector2Int, GameObject>();
        private readonly Dictionary<GameObject, Vector2Int> _objectPositions = new Dictionary<GameObject, Vector2Int>();
        private readonly Dictionary<Vector2Int, GameObject> _units = new Dictionary<Vector2Int, GameObject>();
        private readonly Dictionary<GameObject, Vector2Int> _unitPositions = new Dictionary<GameObject, Vector2Int>();

        private readonly Action<Tile, Tile> _currentTileUpdated;
        public readonly Event<Tile, Tile> CurrentTileUpdated;
        [CanBeNull] public Tile CurrentTile { get; private set; }
        // public Vector2Int CurrentTilePosition => _tilePosition.TryGetValue(CurrentTile, out var position)
        //     ? position
        //     : Vector2Int.zero;

        public Field() {
            CurrentTileUpdated = new Event<Tile, Tile>(out _currentTileUpdated);
        }

        public void Initialize(Vector2Int fieldSize, Vector2Int[] baseLocations, Vector2Int[] unitLocations) {
            var rng = new Rng(Rng.RandomSeed);

            foreach (var position in fieldSize.EnumeratePositions()) {
                var height = GetRandomHeight(rng);
                var tile = Instantiate(_tilePrefab, _tilesParent);
                tile.Initialize(height);
                tile.MouseEnter.Subscribe(OnTileEnter);
                tile.MouseExit.Subscribe(OnTileExit);

                var localPosition2D = Position.Map(position, _tileSize);
                var localPosition = new Vector3(localPosition2D.x, 0f, localPosition2D.y);
                tile.transform.localPosition = localPosition;

                _tiles.Add(position, tile);
                _tilePositions.Add(tile, position);

                void OnTileEnter() => OnCurrentTileUpdated(tile);
                void OnTileExit() => OnCurrentTileUpdated(null);
            }

            foreach (var baseLocation in baseLocations) {
                var @base = Instantiate(_basePrefab, _objectsParent);
                @base.transform.position = _tiles[baseLocation].Center;
                _objects.Add(baseLocation, @base);
                _objectPositions.Add(@base, baseLocation);
            }

            foreach (var unitLocation in unitLocations) {
                var car = Instantiate(_carPrefab, _objectsParent);
                car.transform.position = _tiles[unitLocation].Center;
                _units.Add(unitLocation, car);
                _unitPositions.Add(car, unitLocation);
            }

            _cameraContainer.transform.position = GetFieldCenter(fieldSize);
        }

        private Vector3 GetFieldCenter(Vector2Int fieldSize) {
            var fieldCenter2D = Position.GetFieldCenter(fieldSize, _tileSize);
            return new Vector3(fieldCenter2D.x, 0f, fieldCenter2D.y);
        }

        private void OnCurrentTileUpdated(Tile newCurrent) {
            var current = CurrentTile;
            CurrentTile = newCurrent;
            _currentTileUpdated(current, newCurrent);
        }

        private int GetRandomHeight(Rng rng) {
            return  rng.NextWeightedChoice(_heights, _heightWeights, out _);
        }

        private Vector2Int GetRandomUnoccupiedPosition(Rng rng, Vector2Int fieldSize) {
            while (true) {
                var randomPosition = new Vector2Int(rng.NextInt(0, fieldSize.x - 1), rng.NextInt(0, fieldSize.y - 1));
                if (!_objects.ContainsKey(randomPosition)) {
                    return randomPosition;
                }
            }
        }

        public Vector2Int TilePosition(Tile tile) {
            return _tilePositions[tile];
        }

        public Tile TileAtPosition(Vector2Int position) {
            return _tiles[position];
        }

        public void Clear() {
            foreach (var tile in _tiles.Values) {
                Destroy(tile.gameObject);
            }
            _tiles.Clear();
            _tilePositions.Clear();

            foreach (var @object in _objects.Values) {
                Destroy(@object);
            }
            _objects.Clear();
            _objectPositions.Clear();

            foreach (var unit in _units.Values) {
                Destroy(unit);
            }
            _units.Clear();
            _unitPositions.Clear();
        }

        public void MoveUnit(Vector2Int from, Vector2Int to) {
            var unit = _units[from];
            var tile = _tiles[to];
            unit.transform.position = tile.Center;

            _units[to] = _units[from];
            _units.Remove(from);
        }
    }
}