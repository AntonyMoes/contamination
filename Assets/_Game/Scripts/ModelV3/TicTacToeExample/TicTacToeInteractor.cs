using System;
using System.Collections.Generic;
using GeneralUtils;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class TicTacToeInteractor : MonoBehaviour, ICommandCreator, ICommandReactor {
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private RectTransform _container;
        [SerializeField] private GridLayoutGroup _layout;
        private readonly List<Tile> _tiles = new List<Tile>();
        private readonly Action<GameCommand> _onCommandCreated;
        private TicTacToeState.EMark _currentMark;
        private TicTacToeState _state;

        public Event<GameCommand> OnCommandCreated { get; }

        public TicTacToeInteractor() {
            OnCommandCreated = new Event<GameCommand>(out _onCommandCreated);
        }

        public void InitializeField(TicTacToeState state) {
            foreach (var tile in _tiles) {
                tile.OnClick.RemoveAllListeners();
                Destroy(tile.gameObject);
            }
            _tiles.Clear();

            _state = state;
            _container.sizeDelta = _layout.cellSize * _state.Size + _layout.spacing * (_state.Size - 1);

            for (var i = 0; i < _state.Size; i++) {
                for (var j = 0; j < _state.Size; j++) {
                    var tile = Instantiate(_tilePrefab, _layout.transform);
                    tile.SetMark(state.Field[i][j]);
                    tile.OnClick.AddListener(() => OnTileClick(tile));
                    _tiles.Add(tile);
                }
            }
        }

        public void SetCurrentMark(TicTacToeState.EMark mark) {
            _currentMark = mark;
        }

        private void OnTileClick(Tile tile) {
            var tileIndex = _tiles.IndexOf(tile);
            var (row, column) = TileIndexToRowAndColumn(_state.Size, tileIndex);
            if (_state.Field[row][column] == TicTacToeState.EMark.None) {
                _onCommandCreated(new MarkCommand(_currentMark, row, column));
            }
        }

        public bool ShouldReactToCommand(GameCommand command) {
            return command is MarkCommand;
        }

        public void ReactToCommand(GameCommand command) {
            var markCommand = (MarkCommand) command;
            _tiles[markCommand.Row * _state.Size + markCommand.Column].SetMark(markCommand.Mark);
        }

        private static (int, int) TileIndexToRowAndColumn(int size, int tileIndex) {
            return (tileIndex / size, tileIndex % size);
        }
    }
}
