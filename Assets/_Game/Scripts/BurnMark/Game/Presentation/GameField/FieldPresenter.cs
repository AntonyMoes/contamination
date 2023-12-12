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

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class FieldPresenter : ICommandPresenter, ICommandGenerator, IDisposable {
        private readonly Input _input;
        private readonly Field _field;
        private readonly FieldAccessor _fieldAccessor;
        private readonly IFieldActionUIPresenter _fieldActionUIPresenter;
        private GameDataReadAPI _readAPI;

        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        [CanBeNull] private Tile _selectedTile;
        [CanBeNull] private IReadOnlyEntity _selectedEntity;


        // [CanBeNull] private Vector2Int[] _path;
        [CanBeNull] private IFieldAction _currentAction;

        public FieldPresenter(Input input, Field field, FieldAccessor fieldAccessor,
            IFieldActionUIPresenter fieldActionUIPresenter, Vector2Int fieldSize, Vector2Int[] baseLocations,
            Vector2Int[] unitLocations) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);

            _input = input;
            _field = field;
            _fieldAccessor = fieldAccessor;
            _fieldActionUIPresenter = fieldActionUIPresenter;
            _input.SelectionButton.Subscribe(OnSelection);
            _input.ActionButton.Subscribe(OnAction);
            field.Initialize(fieldSize, baseLocations, unitLocations);
            field.CurrentTileUpdated.Subscribe(OnCurrentTileUpdated);
        }

        public void SetReadAPI(IGameReadAPI api) {
            _readAPI = (GameDataReadAPI) api;
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

            var selectPosition = _field.CurrentTile != null && _fieldAccessor.TryGetUnitAt(_field.TilePosition(_field.CurrentTile), out _)
                ? _field.TilePosition(_field.CurrentTile)
                : (Vector2Int?) null;
            SelectAtPosition(selectPosition);
        }

        private void SelectAtPosition(Vector2Int? position, IReadOnlyEntity entity = null) {
            if (_selectedTile != null) {
                _selectedTile.SetState(Tile.State.None);
            }
            
            if (position is {} selectPosition) {
                var selectedTile = _field.TileAtPosition(selectPosition);
                selectedTile!.SetState(Tile.State.Selected);
                if (selectedTile != null) {
                    _fieldAccessor.TryGetUnitAt(selectPosition, out var selectedUnit);
                    _selectedEntity = entity ?? FieldActions.FieldActions.TrySelectTile(_fieldAccessor, selectPosition);
                    _selectedTile = _selectedEntity != null ? selectedTile : null;
                }
            } else {
                _selectedTile = null;
                _selectedEntity = null;
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

        // private void TryPlotAndDisplayPath() {
        //     ClearPath();
        //
        //     if (_selectedTile == null || _field.CurrentTile == null || _selectedEntity == null) {
        //         return;
        //     }
        //
        //     var path = _fieldAccessor.CalculatePath(_field.TilePosition(_selectedTile),
        //         _field.TilePosition(_field.CurrentTile));
        //     if (path == null) {
        //         return;
        //     }
        //
        //     // var skippedOne = false;
        //     // foreach (var position in path) {
        //     //     if (!skippedOne) {
        //     //         skippedOne = true;
        //     //         continue;
        //     //     }
        //     //
        //     //     _field.TileAtPosition(position).SetState(Tile.State.Selected);
        //     // }
        //
        //     var moveComponent = _selectedEntity!.GetReadOnlyComponent<MoveData>()!;
        //     var moveDistance = moveComponent.Data.RemainingDistance;
        //
        //     for (var i = 1; i < path.Length; i++) {
        //         var position = path[i];
        //         _field.TileAtPosition(position).SetState(i > moveDistance ? Tile.State.Forbidden : Tile.State.Selected);
        //     }
        // }
        //
        // private void ClearPath() {
        //     if (_path == null) {
        //         return;
        //     }
        //
        //     for (var i = 1; i < _path.Length; i++) {
        //         var position = _path[i];
        //         _field.TileAtPosition(position).SetState(Tile.State.None);
        //     }
        //
        //     _path = null;
        // }
        //
        // private void TryMoveUnit(Vector2Int[] path) {
        //     if (_selectedEntity == null || path == null) {
        //         return;
        //     }
        //
        //     var currentPosition = _field.TilePosition(_field.CurrentTile);
        //     if (_fieldAccessor.TryGetUnitAt(currentPosition, out _) ||
        //         _fieldAccessor.TryGetFieldObjectAt(currentPosition, out _)) {
        //         return;
        //     }
        //
        //     var moveData = _selectedEntity.GetReadOnlyComponent<MoveData>()!.Data;
        //     if (path.Length == 1 || path.Length - 1 > moveData.RemainingDistance) {
        //         return;
        //     }
        //
        //     _onCommandGenerated(new MoveCommand {
        //         EntityId = _selectedEntity.Id,
        //         Position = currentPosition
        //     });
        // }

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
            _field.CurrentTileUpdated.Unsubscribe(OnCurrentTileUpdated);
            _field.Clear();
        }
    }
}