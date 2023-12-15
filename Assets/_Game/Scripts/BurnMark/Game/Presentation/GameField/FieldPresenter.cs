using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using GeneralUtils;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class FieldPresenter : ICommandPresenter, ICommandGenerator, IDisposable {
        private readonly Input _input;
        private readonly Field _field;
        private readonly FieldAccessor _fieldAccessor;
        private readonly IFieldActionUIPresenter _fieldActionUIPresenter;
        private GameDataReadAPI _readAPI;

        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        private readonly Action<IReadOnlyEntity> _onEntitySelected;
        public readonly Event<IReadOnlyEntity> OnEntitySelected;
        

        [CanBeNull] private Tile _selectedTile;
        [CanBeNull] private IReadOnlyEntity _selectedEntity;
        [CanBeNull] private IFieldAction _currentAction;

        public FieldPresenter(Input input, Field field, FieldAccessor fieldAccessor,
            IFieldActionUIPresenter fieldActionUIPresenter, Vector2Int fieldSize, Camera uiCamera,
            RectTransform iconsParent) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            OnEntitySelected = new Event<IReadOnlyEntity>(out _onEntitySelected);

            _input = input;
            _field = field;
            _fieldAccessor = fieldAccessor;
            _fieldActionUIPresenter = fieldActionUIPresenter;
            _input.SelectionButton.Subscribe(OnSelection);
            _input.ActionButton.Subscribe(OnAction);
            InitializeField(fieldSize, uiCamera, iconsParent);
            field.CurrentTileUpdated.Subscribe(OnCurrentTileUpdated);
        }

        public void SetReadAPI(IGameReadAPI api) {
            _readAPI = (GameDataReadAPI) api;
        }

        private void InitializeField(Vector2Int fieldSize, Camera uiCamera, RectTransform iconsParent) {
            _field.Initialize(fieldSize, uiCamera, iconsParent);

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
                _field.CreateUnit(position, entity);
            } else {
                _field.DestroyUnit(position);
            }
        }

        private void OnFieldObjectEvent(bool created, Vector2Int position, IReadOnlyEntity entity) {
            if (created) {
                _field.CreateFieldObject(position, entity);
            } else {
                _field.DestroyFieldObject(position);
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

        private void OnSelection(bool buttonDown) {
            if (!buttonDown) {
                OnSelectClick();
            }
        }

        private void OnAction(bool buttonDown) {
            if (buttonDown) {
                UpdateCurrentAction();
            } else {
                ClearCurrentAction(true);
            }
        }

        private void OnSelectClick() {
            if (_selectedTile == _field.CurrentTile) {
                // TODO rework to allow selecting field objects on the same tile as units
                return;
            }

            var selectPosition = _field.CurrentTile != null
                                 // && _fieldAccessor.Units.TryGetValue(_field.TilePosition(_field.CurrentTile), out _)
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
                _currentAction = FieldActions.FieldActions.TryGetAction(_fieldAccessor, _selectedEntity, _field.TilePosition(_field.CurrentTile));
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
                    var unit = _readAPI.Entities.First(e => e.Id == moveCommand.EntityId);
                    var position = unit.GetReadOnlyComponent<PositionData>()!.Data.Position;
                    return new SyncProcess(() => {
                        _field.MoveUnit(position, moveCommand.Position);
                        SelectAtPosition(moveCommand.Position, unit);
                    });
                default:
                    return new DummyProcess();
            }
        }

        public void Dispose() {
            _input.SelectionButton.Unsubscribe(OnSelection);
            _input.ActionButton.Unsubscribe(OnAction);
            _fieldAccessor.OnTerrainEvent.Unsubscribe(OnTerrainEvent);
            _fieldAccessor.OnUnitEvent.Unsubscribe(OnUnitEvent);
            _fieldAccessor.OnFieldObjectEvent.Unsubscribe(OnFieldObjectEvent);
            _field.CurrentTileUpdated.Unsubscribe(OnCurrentTileUpdated);
            _field.Clear();
        }
    }
}