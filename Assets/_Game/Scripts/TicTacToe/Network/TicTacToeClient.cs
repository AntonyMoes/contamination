﻿using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Network;
using _Game.Scripts.TicTacToe.Game;
using UnityEngine;

namespace _Game.Scripts.TicTacToe.Network {
    public class TicTacToeClient : MonoBehaviour {
        [SerializeField] private TicTacToeInteractor _interactor;
        private Client _client;
        private ModelV4.Game _game;

        private void OnEnable() {
            Application.runInBackground = true;
            _client = new Client("localhost", 44444, "a");
            _client.ServerConnection.GetReceiveEvent<GameConfigurationMessage>().Subscribe(OnGameConfigurationMessageReceived);

            _client.Start();
        }

        private void OnGameConfigurationMessageReceived(GameConfigurationMessage message, IPeer serverPeer) {
            _game = ModelV4.Game.StartClient(message, serverPeer, _interactor);

            _game.RegisterPresenter(_interactor);
            _interactor.SetCurrentUser(message.CurrenUser);

            var winChecker = new TicTacToeWinChecker(_game.EventsAPI);
            _game.RegisterGenerator(winChecker);
            _game.RegisterPresenter(winChecker);

            _game.Start();
        }

        private void Update() {
            _client?.PollEvents();
        }

        private void OnDisable() {
            _client.Dispose();
            _client = null;
        }
    }
}
