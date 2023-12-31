using System;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class FieldPresenter : ICommandPresenter, ICommandGenerator, IDisposable, IFrameProcessor {
        private readonly Input _input;
        private readonly Field _field;
        private readonly FieldAccessor _fieldAccessor;
        private readonly IFieldActionUIPresenter _fieldActionUIPresenter;
        private readonly IScheduler _scheduler;

        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        private readonly Action<IReadOnlyEntity> _onEntitySelected;
        public readonly Event<IReadOnlyEntity> OnEntitySelected;

        private int? _player;
        private GameDataReadAPI _readAPI;
        [CanBeNull] private Tile _selectedTile;
        [CanBeNull] private IReadOnlyEntity _selectedEntity;
        [CanBeNull] private IFieldAction _currentAction;
        private bool _dragging;
        private float _dragged;

        public FieldPresenter(Input input, Field field, FieldAccessor fieldAccessor,
            IFieldActionUIPresenter fieldActionUIPresenter, Camera uiCamera, RectTransform iconsParent, IScheduler scheduler) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            OnEntitySelected = new Event<IReadOnlyEntity>(out _onEntitySelected);

            _input = input;
            _field = field;
            _fieldAccessor = fieldAccessor;
            _fieldActionUIPresenter = fieldActionUIPresenter;
            _scheduler = scheduler;
            _scheduler.RegisterFrameProcessor(_field.FieldCamera);
            _scheduler.RegisterFrameProcessor(this);
            _input.Zoom.Subscribe(OnScroll);
            _input.SelectionButton.Subscribe(OnSelection);
            _input.ActionButton.Subscribe(OnAction);
            _field.FieldCamera.Initialize(_input.EdgeVector, _input.MouseMovement);
            InitializeField(uiCamera, iconsParent);
            field.CurrentTileUpdated.Subscribe(OnCurrentTileUpdated);
        }

        public void SetReadAPI(IGameReadAPI api) {
            _readAPI = (GameDataReadAPI) api;
        }

        public void SetPlayer(int? player) {
            _player = player;
        }

        private void InitializeField(Camera uiCamera, RectTransform iconsParent) {
            _field.Initialize(uiCamera, iconsParent);
            _fieldAccessor.FieldSize.Subscribe(_field.SetFieldSize, true);

            _fieldAccessor.OnTerrainEvent.Subscribe(OnTerrainEvent);
            foreach (var (position, terrain) in _fieldAccessor.Terrain) {
                OnTerrainEvent(true, position, terrain);
            }

            _fieldAccessor.OnUnitEvent.Subscribe(OnUnitEvent);
            foreach (var (position, unit) in _fieldAccessor.Units) {
                OnUnitEvent(true, position, unit);
            }
            
            _fieldAccessor.OnFieldObjectEvent.Subscribe(OnFieldObjectEvent);
            foreach (var (position, fieldObject) in _fieldAccessor.FieldObjects) {
                OnUnitEvent(true, position, fieldObject);
            }
        }

        private void OnTerrainEvent(bool created, Vector2Int position, IReadOnlyComponent<TerrainData> terrainComponent) {
            if (created) {
                _field.CreateTile(position, terrainComponent);
            } else {
                _field.DestroyTile(position);
            }
        }

        private void OnUnitEvent(bool created, Vector2Int position, IReadOnlyEntity entity) {
            if (created) {
                var color = entity.GetInReadOnlyOwner<PlayerData>(_readAPI)?.Data.Color;
                _field.CreateUnit(position, entity, color);
            } else {
                _field.DestroyUnit(position);
                OnEntityDestroyed(entity);
            }
        }

        private void OnFieldObjectEvent(bool created, Vector2Int position, IReadOnlyEntity entity) {
            if (created) {
                var color = entity.GetInReadOnlyOwner<PlayerData>(_readAPI)?.Data.Color;
                _field.CreateFieldObject(position, entity, color);
            } else {
                _field.DestroyFieldObject(position);
                OnEntityDestroyed(entity);
            }
        }

        private void OnEntityDestroyed(IReadOnlyEntity entity) {
            if (_selectedEntity == entity) {
                SelectAtPosition(null);
            }
        }

        private void OnCurrentTileUpdated([CanBeNull] Tile oldTile, [CanBeNull] Tile newTile) {
            if (oldTile != null) {
                oldTile.SetState(oldTile == _selectedTile ? Tile.State.Selected : Tile.State.None);
            }

            if (newTile != null) {
                newTile.SetState(newTile == _selectedTile ? Tile.State.Selected : Tile.State.None);
            }

            if (_input.ActionButton.Value) {
                if (newTile != null) {
                    UpdateCurrentAction();
                } else {
                    ClearCurrentAction();
                }
            }
        }

        private void OnScroll(bool scrollUp) {
            _field.FieldCamera.OnZoom(scrollUp);
        }

        private void OnSelection(bool buttonDown) {
            if (!buttonDown) {
                if (!_input.MouseOverUI.Value && _dragged == 0f) {
                    OnSelectClick();
                }
                OnStopDrag();
            } else {
                _dragging = !_input.MouseOverUI.Value;
                if (!_input.MouseOverUI.Value) {
                    OnStartDrag();
                }
            }
        }

        private void OnAction(bool buttonDown) {
            if (buttonDown && !_input.MouseOverUI.Value) {
                UpdateCurrentAction();
            } else if (!buttonDown) {
                ClearCurrentAction(true);
            }
        }

        private void OnStartDrag() {
            _dragging = true;
            _dragged = 0f;
            _field.FieldCamera.ToggleDrag(true);
        }

        private void OnStopDrag() {
            _dragging = false;
            _field.FieldCamera.ToggleDrag(false);
        }

        private void OnSelectClick() {
            if (_selectedTile == _field.CurrentTile) {
                // TODO rework to allow selecting field objects on the same tile as units
                return;
            }

            var selectPosition = _field.CurrentTile != null
                ? _field.TilePosition(_field.CurrentTile)
                : (Vector2Int?) null;
            SelectAtPosition(selectPosition);
        }

        private void SelectAtPosition(Vector2Int? position, IReadOnlyEntity entity = null) {
            if (_selectedTile != null) {
                _selectedTile.SetState(Tile.State.None);
            }

            var previousSelectedEntity = _selectedEntity;
            if (position is {} selectPosition) {
                var selectedTile = _field.TileAtPosition(selectPosition);
                if (selectedTile != null) {
                    _selectedEntity = entity ?? FieldActions.FieldActions.TrySelectTile(_fieldAccessor, selectPosition);
                    _selectedTile = _selectedEntity != null ? selectedTile : null;
                    if (_selectedTile != null) {
                        _selectedTile.SetState(Tile.State.Selected);
                    }
                }
            } else {
                _selectedTile = null;
                _selectedEntity = null;
            }

            if (_selectedEntity != previousSelectedEntity) {
                _onEntitySelected(_selectedEntity);
            }
        }

        private void UpdateCurrentAction() {
            ClearCurrentAction();

            if (_field.CurrentTile != null) {
                _currentAction = FieldActions.FieldActions.TryGetAction(_fieldAccessor, _player, _selectedEntity, _field.TilePosition(_field.CurrentTile));
                _currentAction?.DrawPreview(_field, _fieldActionUIPresenter);
            }
        }

        private void ClearCurrentAction(bool apply = false) {
            _currentAction?.ClearPreview(_field, _fieldActionUIPresenter);

            if (apply && _currentAction?.Command is {} command) {
                _onCommandGenerated(command);
            }

            _currentAction = null;
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            switch (generatedCommand) {
                case MoveCommand moveCommand:
                    var unit = _readAPI.Entities[moveCommand.EntityId];
                    var position = unit.GetReadOnlyComponent<PositionData>()!.Data.Position;
                    return new SyncProcess(() => {
                        _field.MoveUnit(position, moveCommand.Position);
                        SelectAtPosition(moveCommand.Position, unit);
                    });
                default:
                    return new DummyProcess();
            }
        }

        public void ProcessFrame(float deltaTime) {
            if (!_dragging) {
                return;
            }

            _dragged += _input.MouseMovement.Value.magnitude;
        }

        public void Dispose() {
            _scheduler.UnregisterFrameProcessor(_field.FieldCamera);
            _scheduler.UnregisterFrameProcessor(this);
            _input.Zoom.Unsubscribe(OnScroll);
            _input.SelectionButton.Unsubscribe(OnSelection);
            _input.ActionButton.Unsubscribe(OnAction);
            _fieldAccessor.FieldSize.Unsubscribe(_field.SetFieldSize);
            _fieldAccessor.OnTerrainEvent.Unsubscribe(OnTerrainEvent);
            _fieldAccessor.OnUnitEvent.Unsubscribe(OnUnitEvent);
            _fieldAccessor.OnFieldObjectEvent.Unsubscribe(OnFieldObjectEvent);
            _field.CurrentTileUpdated.Unsubscribe(OnCurrentTileUpdated);
            _field.Clear();
        }
    }
}