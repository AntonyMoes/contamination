using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Pathfinding;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;
using JetBrains.Annotations;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Data {
    public class FieldAccessor {
        private readonly GameDataReadAPI _readAPI;
        private readonly GameDataEventsAPI _eventsAPI;
        private readonly IPathFindingAlgorithm _algorithm;

        private readonly Dictionary<Vector2Int, IReadOnlyComponent<TerrainData>> _terrain =
            new Dictionary<Vector2Int, IReadOnlyComponent<TerrainData>>();
        public IDictionary<Vector2Int, IReadOnlyComponent<TerrainData>> Terrain => _terrain;

        private readonly Dictionary<Vector2Int, IReadOnlyEntity> _fieldObjects =
            new Dictionary<Vector2Int, IReadOnlyEntity>();
        public IDictionary<Vector2Int, IReadOnlyEntity> FieldObjects => _fieldObjects;

        private readonly Dictionary<Vector2Int, IReadOnlyEntity> _units =
            new Dictionary<Vector2Int, IReadOnlyEntity>();
        public IDictionary<Vector2Int, IReadOnlyEntity> Units => _units;

        private readonly Action<bool, Vector2Int, IReadOnlyComponent<TerrainData>> _onTerrainEvent;
        public readonly Event<bool, Vector2Int, IReadOnlyComponent<TerrainData>> OnTerrainEvent;

        private readonly Action<bool, Vector2Int, IReadOnlyEntity> _onFieldObjectEvent;
        public readonly Event<bool, Vector2Int, IReadOnlyEntity> OnFieldObjectEvent;
        
        private readonly Action<bool, Vector2Int, IReadOnlyEntity> _onUnitEvent;
        public readonly Event<bool, Vector2Int, IReadOnlyEntity> OnUnitEvent;

        public FieldAccessor(GameDataReadAPI readAPI, GameDataEventsAPI eventsAPI, IPathFindingAlgorithm algorithm) {
            OnTerrainEvent = new Event<bool, Vector2Int, IReadOnlyComponent<TerrainData>>(out _onTerrainEvent);
            OnFieldObjectEvent = new Event<bool, Vector2Int, IReadOnlyEntity>(out _onFieldObjectEvent);
            OnUnitEvent = new Event<bool, Vector2Int, IReadOnlyEntity>(out _onUnitEvent);

            _readAPI = readAPI;
            _eventsAPI = eventsAPI;
            _algorithm = algorithm;
            _algorithm.SetAccessor(this);

            _eventsAPI.OnEntityCreated.Subscribe(OnEntityCreated);
            _eventsAPI.OnEntityDestroyed.Subscribe(OnEntityDestroyed);
            
            _eventsAPI.GetComponentUpdateEvent<PositionData>().Subscribe(OnPositionChanged);

            foreach (var entity in _readAPI.Entities) {
                OnEntityCreated(entity);
            }
        }

        private void OnPositionChanged(PositionData? oldPosition, IReadOnlyComponent<PositionData> newPosition) {
            if (!(oldPosition is { } oldPositionData)) {
                return;
            }

            var oldPos = oldPositionData.Position;
            if (_units[oldPos].Id != newPosition.ReadOnlyEntity.Id) {
                throw new Exception();
            }

            _units[newPosition.Data.Position] = _units[oldPos];
            _units.Remove(oldPos);
        }

        private void OnEntityCreated(IReadOnlyEntity entity) {
            if (!(entity.GetReadOnlyComponent<PositionData>() is { } positionData)) {
                return;
            }

            var position = positionData.Data.Position;
            if (Entities.Terrain.IsTerrain(entity, out var terrainComponent)) {
                _terrain.Add(position, terrainComponent);
                _onTerrainEvent(true, position, terrainComponent);
            } else if (entity.GetReadOnlyComponent<FieldObjectData>() != null) {
                _fieldObjects.Add(position, entity);
                _onFieldObjectEvent(true, position, entity);
            } else if (Unit.IsUnit(entity, out _)) {
                _units.Add(position, entity);
                _onUnitEvent(true, position, entity);
            }
        }

        private void OnEntityDestroyed(IReadOnlyEntity entity) {
            if (!(entity.GetReadOnlyComponent<PositionData>() is { } positionData)) {
                return;
            }

            var position = positionData.Data.Position;
            if (entity.GetReadOnlyComponent<TerrainData>() is {} terrainComponent) {
                _terrain.Remove(position);
                _onTerrainEvent(false, position, terrainComponent);
            } else if (entity.GetReadOnlyComponent<FieldObjectData>() != null) {
                _fieldObjects.Remove(position);
                _onFieldObjectEvent(false, position, entity);
            } else if (entity.GetReadOnlyComponent<UnitData>() != null) {
                _units.Remove(position);
                _onUnitEvent(false, position, entity);
            }
        }

        public IReadOnlyList<IReadOnlyEntity> TryGetEntitiesAt(Vector2Int position) {
            var result = new List<IReadOnlyEntity>();
            if (_units.TryGetValue(position, out var unit)) {
                result.Add(unit);
            }

            if (_fieldObjects.TryGetValue(position, out var fieldObject)) {
                result.Add(fieldObject);
            }

            return result;
        }

        [CanBeNull]
        public Vector2Int[] CalculatePath(IReadOnlyEntity entity, Vector2Int from, Vector2Int to) {
            return _algorithm.CalculatePath(entity, from, to);
        }
    }
}