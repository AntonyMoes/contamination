﻿using System;
using System.Linq;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.NetTicTacToeExample.Commands;
using _Game.Scripts.ModelV4.NetTicTacToeExample.Data;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class TicTacToeWinChecker : ICommandPresenter, ICommandGenerator {
        private readonly Action<GameCommand> _onCommandGenerated;
        private GameDataReadAPI _readApi;

        public TicTacToeWinChecker(GameDataEventsAPI api) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            api.GetComponentUpdateEvent<MarkData>().Subscribe(OnMarkUpdated);
        }

        private void OnMarkUpdated(MarkData oldData, IReadOnlyComponent<MarkData> component) {
            var data = component.Data;
            var settings = _readApi.Entities.GetSettings();
            var position = component.Entity.GetReadOnlyComponent<PositionData>().Data;
            if (CheckRow(data.Mark, position.Row, settings.Size)
                || CheckColumn(data.Mark, position.Column, settings.Size)
                || CheckDiagonal(data.Mark, position.Row, position.Column, settings.Size)) {
                _onCommandGenerated(new WinCommand {
                    Winner = settings.PlayerPerMark[data.Mark]
                });
            } else if (_readApi.Entities
                .Select(e => e.GetReadOnlyComponent<MarkData>())
                .Where(c => c != null)
                .All(c => c.Data.Mark != MarkData.EMark.None)) {
                _onCommandGenerated(new WinCommand {
                    Winner = -1
                });
            }
        }

        private bool CheckRow(MarkData.EMark mark, int row, int size) {
            for (var i = 0; i < size; i++) {
                if (_readApi.Entities.AtCoordinates(row, i).Data.Mark != mark) {
                    return false;
                }
            }

            return true;
        }

        private bool CheckColumn(MarkData.EMark mark, int column, int size) {
            for (var i = 0; i < size; i++) {
                if (_readApi.Entities.AtCoordinates(i, column).Data.Mark != mark) {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDiagonal(MarkData.EMark mark, int row, int column, int size) {
            var columnSelectors = new Func<int, int>[] {
                r => r,
                r => size - r - 1
            };

            var selector = columnSelectors.FirstOrDefault(s => s(row) == column);
            if (selector == null) {
                return false;
            }

            for (var i = 0; i < size; i++) {
                if (_readApi.Entities.AtCoordinates(i, selector(i)).Data.Mark != mark) {
                    return false;
                }
            }

            return true;
        }

        public void SetReadAPI(GameDataReadAPI api) {
            _readApi = api;
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            if (!(generatedCommand is WinCommand winCommand)) {
                return null;
            }

            return new SyncProcess(() => {
                var winner = _readApi.UserSequence.FirstOrDefault(user => user.Id == winCommand.Winner);
                var message = winner != null
                    ? winner.Name + " WON!!!"
                    : "NOBODY WON THIS TIME";
                Debug.LogWarning(message);
            });
        }

        public Event<GameCommand> OnCommandGenerated { get; }
    }
}
