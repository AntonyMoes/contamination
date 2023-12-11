using System;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using GeneralUtils.Processes;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class GamePresenter : ICommandPresenter, IDisposable {
        private readonly ProxyCommandGenerator _proxy;
        private readonly int _player;
        private readonly PlayerUI _playerUI;
        private readonly GameFieldPresenter _fieldPresenter;

        public GamePresenter(ProxyCommandGenerator proxy, int player, PlayerUI playerUI,
            GameDataEventsAPI eventsAPI, Action onPlayerClosedGame, GameFieldPresenter gameFieldPresenter) {
            _proxy = proxy;
            _player = player;
            _playerUI = playerUI;
            _fieldPresenter = gameFieldPresenter;
            _playerUI.Initialize(_player, eventsAPI, EndTurn, EndGame, onPlayerClosedGame);
            _playerUI.gameObject.SetActive(true);
            _fieldPresenter.OnCommandGenerated.Subscribe(_proxy.GenerateCommand);
        }

        public void Dispose() {
            _playerUI.gameObject.SetActive(false);
            _fieldPresenter.OnCommandGenerated.Unsubscribe(_proxy.GenerateCommand);
            _fieldPresenter.Dispose();
        }

        public void SetReadAPI(IGameReadAPI api) {
            _playerUI.SetReadAPI((GameDataReadAPI) api);
            _fieldPresenter.SetReadAPI(api);
        }

        private void EndTurn() {
            _proxy.GenerateCommand(new EndTurnCommand());
        }

        private void EndGame() {
            _proxy.GenerateCommand(new TestEndGameCommand());
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            Debug.Log($"PRESENTING {generatedCommand}");

            return ParallelProcess.From(PerformPresent(generatedCommand),
                _fieldPresenter.PresentCommand(generatedCommand));
            
        }

        private Process PerformPresent(GameCommand generatedCommand) {
            switch (generatedCommand) {
                case TestEndGameCommand endGameCommand:
                    return new SyncProcess(() => _playerUI.OnGameEnded(endGameCommand.Winner == _player));
                default:
                    return new DummyProcess();
            }
        }
    }
}