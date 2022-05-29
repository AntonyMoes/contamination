using System;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class TicTacToeMechanics : ICommandReactor {
        private readonly TicTacToeState _state;
        private readonly TicTacToeState.EMark[] _order;
        private readonly Action<TicTacToeState.EMark> _markSetter;
        private readonly Action<TicTacToeState.EMark> _onWin;

        private int _orderIndex = -1;

        public TicTacToeMechanics(TicTacToeState state, TicTacToeState.EMark[] order,
            Action<TicTacToeState.EMark> markSetter, Action<TicTacToeState.EMark> onWin) {
            _state = state;
            _order = order;
            _markSetter = markSetter;
            _onWin = onWin;

            UpdateMark();
        }

        private void UpdateMark() {
            _orderIndex = (_orderIndex + 1) % _order.Length;
            _markSetter(_order[_orderIndex]);
        }

        public bool ShouldReactToCommand(GameCommand command) {
            return command is MarkCommand;
        }

        public void ReactToCommand(GameCommand command) {
            var markCommand = (MarkCommand) command;
            var mark = markCommand.Mark;
            var row = markCommand.Row;
            var column = markCommand.Column;

            if (CheckRow(mark, row) || CheckColumn(mark, column) || CheckDiagonal(mark, row, column)) {
                _onWin(mark);
                return;
            }

            UpdateMark();
        }

        private bool CheckRow(TicTacToeState.EMark mark, int row) {
            for (var i = 0; i < _state.Size; i++) {
                if (_state.Field[row][i] != mark) {
                    return false;
                }
            }

            return true;
        }

        private bool CheckColumn(TicTacToeState.EMark mark, int column) {
            for (var i = 0; i < _state.Size; i++) {
                if (_state.Field[i][column] != mark) {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDiagonal(TicTacToeState.EMark mark, int row, int column) {
            var columnSelectors = new Func<int, int>[] {
                r => r,
                r => _state.Size - r - 1
            };

            var selector = columnSelectors.FirstOrDefault(s => s(row) == column);
            if (selector == null) {
                return false;
            }

            for (var i = 0; i < _state.Size; i++) {
                if (_state.Field[i][selector(i)] != mark) {
                    return false;
                }
            }

            return true;
        }
    }
}
