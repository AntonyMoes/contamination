using System;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class GamePresenter : ICommandGenerator, ICommandPresenter {
        private readonly int _player;
        private readonly PlayerUI _playerUI;

        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        public GamePresenter(int player, PlayerUI playerUI) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);

            _player = player;
            _playerUI = playerUI;
        }

        public void Start(GameDataEventsAPI eventsAPI, Action onPlayerClosedGame) {
            _playerUI.Initialize(_player, eventsAPI, EndTurn, EndGame, onPlayerClosedGame);
            _playerUI.gameObject.SetActive(true);
        }

        public void Stop() {
            _playerUI.gameObject.SetActive(false);
        }

        public void SetReadAPI(IGameReadAPI api) {
            _playerUI.SetReadAPI((GameDataReadAPI) api);
        }

        private void EndTurn() {
            _onCommandGenerated(new EndTurnCommand());
        }

        private void EndGame() {
            _onCommandGenerated(new TestEndGameCommand());
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            Debug.Log($"PRESENTING {generatedCommand}");

            switch (generatedCommand) {
                case TestEndGameCommand endGameCommand:
                    return new SyncProcess(() => _playerUI.OnGameEnded(endGameCommand.Winner == _player));
                default:
                    return new DummyProcess();
            }
        }
    }
}