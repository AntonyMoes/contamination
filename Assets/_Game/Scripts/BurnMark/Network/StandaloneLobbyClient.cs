using _Game.Scripts.BurnMark.Game;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.UI;
using _Game.Scripts.Lobby;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Network;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Network {
    public class StandaloneLobbyClient : MonoBehaviour {
        [SerializeField] private PlayerUI _playerUI;
        [SerializeField] private LobbyClientInterface _lobbyClientInterface;

        private Client _client;
        private LobbyClient<RoomSettings> _lobbyClient;
        private ModelV4.Game _game;

        private void OnEnable() {
            Application.runInBackground = true;
            _client = new Client("localhost", 44444, "a");

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
            _game = GameStarter.StartClientGame(message, serverPeer, _playerUI, OnClientClosedGame);
            _game.EventsAPI.OnGameEnded.Subscribe(OnGameEnded);
            _game.Start();
        }

        private void OnGameEnded() {
            _game.EventsAPI.OnGameEnded.Unsubscribe(OnGameEnded);
            _game.Dispose();
            _game = null;
        }

        private void OnClientClosedGame() {
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
