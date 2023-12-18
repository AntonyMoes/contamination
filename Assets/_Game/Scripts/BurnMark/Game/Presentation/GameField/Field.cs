using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Field : MonoBehaviour {
        [Header("Tiles")]
        [SerializeField] private Transform _tilesParent;
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private float _tileSize;

        [Header("Objects")]
        [SerializeField] private Transform _objectsParent;

        [Header("Icons")]
        [SerializeField] private EntityIcon _iconPrefab;

        [Header("RandomTest")]
        [FormerlySerializedAs("_tileWeights")]
        [SerializeField] private float[] _heightWeights;
        [SerializeField] private int[] _heights;

        [Header("Camera")]
        [SerializeField] private GameObject _cameraContainer;
        [SerializeField] private Camera _fieldCamera;

        private readonly Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();
        private readonly Dictionary<Tile, Vector2Int> _tilePositions = new Dictionary<Tile, Vector2Int>();
        private readonly Dictionary<Vector2Int, GameObject> _objects = new Dictionary<Vector2Int, GameObject>();
        private readonly Dictionary<GameObject, Vector2Int> _objectPositions = new Dictionary<GameObject, Vector2Int>();
        private readonly Dictionary<Vector2Int, GameObject> _units = new Dictionary<Vector2Int, GameObject>();
        private readonly Dictionary<GameObject, Vector2Int> _unitPositions = new Dictionary<GameObject, Vector2Int>();
        private readonly Dictionary<GameObject, EntityIcon> _icons = new Dictionary<GameObject, EntityIcon>();

        private readonly Action<Tile, Tile> _currentTileUpdated;
        public readonly Event<Tile, Tile> CurrentTileUpdated;

        private Camera _uiCamera;
        private RectTransform _iconsParent;

        [CanBeNull] public Tile CurrentTile { get; private set; }

        public Field() {
            CurrentTileUpdated = new Event<Tile, Tile>(out _currentTileUpdated);
        }

        public void Initialize(Vector2Int fieldSize, Camera uiCamera, RectTransform iconsParent) {
            _cameraContainer.transform.position = GetFieldCenter(fieldSize);
            _uiCamera = uiCamera;
            _iconsParent = iconsParent;
        }

        public void CreateTile(Vector2Int position, IReadOnlyComponent<TerrainData> terrain) {
            // TODO config
            var tile = Instantiate(_tilePrefab, _tilesParent);
            tile.Initialize(terrain.Data.Height);
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

        public void DestroyTile(Vector2Int position) {
            var tile = _tiles[position];
            Destroy(tile.gameObject);
            _tiles.Remove(position);
            _tilePositions.Remove(tile);
        }

        public void CreateUnit(Vector2Int position, IReadOnlyEntity entity) {
            var config = entity.GetReadOnlyComponent<UnitData>()!.Data.Config;
            var unit = Instantiate(config.Prefab, _objectsParent);
            unit.transform.position = _tiles[position].Center;
            _units.Add(position, unit);
            _unitPositions.Add(unit, position);
            CreateIcon(entity, unit, unit.transform, config.Icon);
        }

        public void DestroyUnit(Vector2Int position) {
            var unit = _units[position];
            DestroyIcon(unit);
            Destroy(unit.gameObject);
            _units.Remove(position);
            _unitPositions.Remove(unit);
        }

        public void CreateFieldObject(Vector2Int position, IReadOnlyEntity entity) {
            var config = entity.GetReadOnlyComponent<FieldObjectData>()!.Data.Config;
            var fieldObject = Instantiate(config.Prefab, _objectsParent);
            fieldObject.transform.position = _tiles[position].Center;
            _objects.Add(position, fieldObject);
            _objectPositions.Add(fieldObject, position);
            CreateIcon(entity, fieldObject, fieldObject.transform, config.Icon);
        }

        public void DestroyFieldObject(Vector2Int position) {
            var fieldObject = _objects[position];
            DestroyIcon(fieldObject);
            Destroy(fieldObject.gameObject);
            _objects.Remove(position);
            _objectPositions.Remove(fieldObject);
        }

        private void CreateIcon(IReadOnlyEntity entity, GameObject entityObject, Transform target,
            Sprite iconSprite) {
            var icon = Instantiate(_iconPrefab, _iconsParent);
            icon.Initialize(target, iconSprite, _fieldCamera, _uiCamera, entity.GetReadOnlyComponent<HealthData>());
            _icons.Add(entityObject, icon);
        }

        private void DestroyIcon(GameObject entity) {
            var icon = _icons[entity];
            icon.Clear();
            Destroy(icon.gameObject);
            _icons.Remove(entity);
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