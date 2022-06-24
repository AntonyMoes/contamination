using _Game.Scripts.Lobby;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.Network;
using _Game.Scripts.Network;
using UnityEngine;

namespace _Game.Scripts.TicTacToe {
    public class StandaloneLobbyClient : MonoBehaviour {
        [SerializeField] private TicTacToeInteractor _interactor;
        [SerializeField] private TicTacToeLobbyClientInterface _lobbyClientInterface;

        private Client _client;
        private LobbyClient<TicTacToeRoomSettings> _lobbyClient;
        private Game _game;

        private void OnEnable() {
            Application.runInBackground = true;
            _client = new Client("localhost", 44444, "a");
            // _client.ServerConnection.GetReceiveEvent<GameConfigurationMessage>().Subscribe(OnGameConfigurationMessageReceived);

            _client.Start();
            _client.OnConnected.Subscribe(OnConnected);
        }

        private void OnConnected(bool connected) {
            _lobbyClient = new LobbyClient<TicTacToeRoomSettings>(_client.ServerConnection);
            if (connected) {
                _lobbyClientInterface.StartLobby(_lobbyClient);
            } else {
                _lobbyClientInterface.StopLobby();
            }
        }

        // private void OnGameConfigurationMessageReceived(GameConfigurationMessage message, IPeer serverPeer) {
        //     _game = Game.StartClient(message, serverPeer, _interactor);
        //
        //     _game.RegisterPresenter(_interactor);
        //     _interactor.SetCurrentUser(message.CurrenUser);
        //
        //     var winChecker = new TicTacToeWinChecker(_game.EventsAPI);
        //     _game.RegisterGenerator(winChecker);
        //     _game.RegisterPresenter(winChecker);
        //
        //     _game.Start();
        // }

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
