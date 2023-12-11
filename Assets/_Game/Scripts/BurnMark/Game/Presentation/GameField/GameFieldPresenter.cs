using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using GeneralUtils;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class GameFieldPresenter : ICommandPresenter, ICommandGenerator{
        private Field _field;
        private FieldAccessor _fieldAccessor;
        private GameDataReadAPI _readAPI;

        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        [CanBeNull] private Tile _selectedTile;

        public GameFieldPresenter() {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
        }

        public void Start(Field field, FieldAccessor fieldAccessor, Vector2Int fieldSize, Vector2Int[] baseLocations,
            Vector2Int[] unitLocations) {
            _field = field;
            _fieldAccessor = fieldAccessor;
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
        }

        public void OnSelectClick() {
            if (_selectedTile == _field.CurrentTile) {
                // TODO rework to allow selecting field objects on the same tile as units
                return;
            }

            var selectPosition = _field.CurrentTile != null && _fieldAccessor.TryGetUnitAt(_field.TilePosition(_field.CurrentTile), out _)
                ? _field.TilePosition(_field.CurrentTile)
                : (Vector2Int?) null;
            SelectAtPosition(selectPosition);
        }

        private void SelectAtPosition(Vector2Int? position) {
            if (_selectedTile != null) {
                _selectedTile.SetState(Tile.State.None);
            }

            if (position is {} selectPosition) {
                _selectedTile = _field.TileAtPosition(selectPosition);
                _selectedTile!.SetState(Tile.State.Selected);
            } else {
                _selectedTile = null;
            }
        }

        public void OnActionClick() {
            if (_selectedTile == null ||
                !_fieldAccessor.TryGetUnitAt(_field.TilePosition(_selectedTile), out var unit)) {
                return;
            }

            if (_field.CurrentTile == null) {
                return;
            }

            var currentPosition = _field.TilePosition(_field.CurrentTile);
            if (_fieldAccessor.TryGetUnitAt(currentPosition, out _) ||
                _fieldAccessor.TryGetFieldObjectAt(currentPosition, out _)) {
                return;
            }

            _onCommandGenerated(new MoveCommand {
                EntityId = unit.Id,
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
                        SelectAtPosition(moveCommand.Position);
                    });
                default:
                    return new DummyProcess();
            }
        }

        public void Clear() {
            _field.Clear();
        }
    }
}