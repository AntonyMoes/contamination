﻿using _Game.Scripts.BurnMark.Game;
using _Game.Scripts.BurnMark.UI;
using _Game.Scripts.Lobby;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Network;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Network {
    public class StandaloneLobbyClient : MonoBehaviour {
        [SerializeField] private TicTacToeInteractor _interactor;
        [SerializeField] private LobbyClientInterface _lobbyClientInterface;

        private Client _client;
        private LobbyClient<RoomSettings> _lobbyClient;
        private ModelV4.Game _game;
        private int _currentUserId;
        private int _winnerId;

        private void OnEnable() {
            Application.runInBackground = true;
            _client = new Client("localhost", 44444, "a");
            // _client.ServerConnection.GetReceiveEvent<GameConfigurationMessage>().Subscribe(OnGameConfigurationMessageReceived);

            _client.Start();
            _client.OnConnected.Subscribe(OnConnected);
        }

        private void OnConnected(bool connected) {
            _lobbyClient = new LobbyClient<RoomSettings>(_client.ServerConnection);
            if (connected) {
                _lobbyClientInterface.StartLobby(_lobbyClient, PrepareToStartGame);
            } else {
                _lobbyClientInterface.StopLobby();
            }
        }

        private void PrepareToStartGame() {
            _lobbyClientInterface.StopLobby();
            _client.ServerConnection.GetReceiveEvent<GameConfigurationMessage>().Subscribe(OnGameConfigurationMessageReceived);
        }

        private void OnGameConfigurationMessageReceived(GameConfigurationMessage message, IPeer serverPeer) {
            _client.ServerConnection.GetReceiveEvent<GameConfigurationMessage>().Unsubscribe(OnGameConfigurationMessageReceived);
            _game = ModelV4.Game.StartClient(message, serverPeer, _interactor);

            _game.RegisterPresenter(_interactor);
            _interactor.SetCurrentUser(message.CurrenUser);
            _interactor.gameObject.SetActive(true);

            _currentUserId = message.CurrenUser;

            var winChecker = new TicTacToeWinChecker(_game.EventsAPI, winnerId => _winnerId = winnerId);
            _game.RegisterGenerator(winChecker);
            _game.RegisterPresenter(winChecker);

            _game.EventsAPI.OnGameEnded.Subscribe(OnGameEnded);

            _game.Start();
        }

        private void OnGameEnded() {
            _game.EventsAPI.OnGameEnded.Unsubscribe(OnGameEnded);
            _game.Dispose();
            _game = null;

            _interactor.gameObject.SetActive(false);

            Debug.LogWarning(_currentUserId == _winnerId ? "NICE" : "NO LUCK"); // TODO proper demonstration with modal window

            _lobbyClientInterface.StartLobby(_lobbyClient, PrepareToStartGame);
        }

        private void Update() {
            _client?.PollEvents();
        }

        private void OnDisable() {
            _lobbyClient.Stop();
            _client.Dispose();
            _client = null;
        }
    }
}
