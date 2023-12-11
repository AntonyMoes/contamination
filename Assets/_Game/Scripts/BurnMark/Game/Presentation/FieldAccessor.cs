using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class FieldAccessor {
        private readonly GameDataReadAPI _readAPI;
        private readonly GameDataEventsAPI _eventsAPI;

        private readonly Dictionary<Vector2Int, IReadOnlyEntity> _terrain =
            new Dictionary<Vector2Int, IReadOnlyEntity>();
        private readonly Dictionary<Vector2Int, IReadOnlyEntity> _fieldObjects =
            new Dictionary<Vector2Int, IReadOnlyEntity>();
        private readonly Dictionary<Vector2Int, IReadOnlyEntity> _units =
            new Dictionary<Vector2Int, IReadOnlyEntity>();

        public FieldAccessor(GameDataReadAPI readAPI, GameDataEventsAPI eventsAPI) {
            _readAPI = readAPI;
            _eventsAPI = eventsAPI;
            
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
            if (entity.GetReadOnlyComponent<TerrainData>() != null) {
                _terrain.Add(position, entity);
            } else if (entity.GetReadOnlyComponent<FieldObjectData>() != null) {
                _fieldObjects.Add(position, entity);
            } else if (entity.GetReadOnlyComponent<UnitData>() != null) {
                _units.Add(position, entity);
            }
        }

        private void OnEntityDestroyed(IReadOnlyEntity entity) {
            if (!(entity.GetReadOnlyComponent<PositionData>() is { } positionData)) {
                return;
            }

            var position = positionData.Data.Position;
            if (entity.GetReadOnlyComponent<TerrainData>() != null) {
                _terrain.Remove(position);
            } else if (entity.GetReadOnlyComponent<FieldObjectData>() != null) {
                _fieldObjects.Remove(position);;
            } else if (entity.GetReadOnlyComponent<UnitData>() != null) {
                _units.Remove(position);
            }
        }

        public bool TryGetTerrainAt(Vector2Int position, out IReadOnlyEntity entity) {
            return _terrain.TryGetValue(position, out entity);
        }

        public bool TryGetFieldObjectAt(Vector2Int position, out IReadOnlyEntity entity) {
            return _fieldObjects.TryGetValue(position, out entity);
        }

        public bool TryGetUnitAt(Vector2Int position, out IReadOnlyEntity entity) {
            return _units.TryGetValue(position, out entity);
        }
    }
}