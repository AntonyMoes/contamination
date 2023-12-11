using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using GeneralUtils;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class GameFieldPresenter : ICommandPresenter, ICommandGenerator, IDisposable {
        private readonly Input _input;
        private readonly Field _field;
        private readonly FieldAccessor _fieldAccessor;
        private GameDataReadAPI _readAPI;

        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        [CanBeNull] private Tile _selectedTile;
        [CanBeNull] private IReadOnlyEntity _selectedUnit;
        [CanBeNull] private Vector2Int[] _path;

        public GameFieldPresenter(Input input, Field field, FieldAccessor fieldAccessor, Vector2Int fieldSize,
            Vector2Int[] baseLocations, Vector2Int[] unitLocations) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);

            _input = input;
            _field = field;
            _fieldAccessor = fieldAccessor;
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
                newTile.SetState(newTile == _selectedTile ? Tile.State.Selected : Tile.State.Highlighted);
            }

            if (_input.ActionButton.Value) {
                if (newTile != null) {
                    TryPlotAndDisplayPath();
                } else {
                    ClearPath();
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
                TryPlotAndDisplayPath();
            } else {
                var path = _path;
                ClearPath();
                TryMoveUnit(path);
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

        private void SelectAtPosition(Vector2Int? position, IReadOnlyEntity unit = null) {
            if (_selectedTile != null) {
                _selectedTile.SetState(Tile.State.None);
            }
            
            if (position is {} selectPosition) {
                _selectedTile = _field.TileAtPosition(selectPosition);
                _selectedTile!.SetState(Tile.State.Selected);
                if (_selectedTile != null) {
                    _fieldAccessor.TryGetUnitAt(selectPosition, out var selectedUnit);
                    _selectedUnit = unit ?? selectedUnit;
                }
            } else {
                _selectedTile = null;
                _selectedUnit = unit;
            }
        }

        private void TryPlotAndDisplayPath() {
            ClearPath();

            if (_selectedTile == null || _field.CurrentTile == null || _selectedUnit == null) {
                return;
            }

            _path = _fieldAccessor.CalculatePath(_field.TilePosition(_selectedTile),
                _field.TilePosition(_field.CurrentTile));
            if (_path == null) {
                return;
            }

            var skippedOne = false;
            foreach (var position in _path) {
                if (!skippedOne) {
                    skippedOne = true;
                    continue;
                }

                _field.TileAtPosition(position).SetState(Tile.State.Selected);
            }

            var moveComponent = _selectedUnit!.GetReadOnlyComponent<MoveData>()!;
            var moveDistance = moveComponent.Data.RemainingDistance;

            for (var i = 1; i < _path.Length; i++) {
                var position = _path[i];
                _field.TileAtPosition(position).SetState(i > moveDistance ? Tile.State.Forbidden : Tile.State.Selected);
            }
        }

        private void ClearPath() {
            if (_path == null) {
                return;
            }

            for (var i = 1; i < _path.Length; i++) {
                var position = _path[i];
                _field.TileAtPosition(position).SetState(Tile.State.None);
            }

            _path = null;
        }

        private void TryMoveUnit(Vector2Int[] path) {
            if (_selectedUnit == null || path == null) {
                return;
            }

            var currentPosition = _field.TilePosition(_field.CurrentTile);
            if (_fieldAccessor.TryGetUnitAt(currentPosition, out _) ||
                _fieldAccessor.TryGetFieldObjectAt(currentPosition, out _)) {
                return;
            }

            var moveData = _selectedUnit.GetReadOnlyComponent<MoveData>()!.Data;
            if (path.Length == 1 || path.Length - 1 > moveData.RemainingDistance) {
                return;
            }

            _onCommandGenerated(new MoveCommand {
                EntityId = _selectedUnit.Id,
                Position = currentPosition
            });
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
            _field.CurrentTileUpdated.Unsubscribe(OnCurrentTileUpdated);
            _field.Clear();
        }
    }
}