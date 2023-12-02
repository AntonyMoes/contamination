using _Game.Scripts.ModelV4;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Network;
using _Game.Scripts.TicTacToe.Commands;
using _Game.Scripts.TicTacToe.Data;
using UnityEngine;

namespace _Game.Scripts.TicTacToe {
    public class TicTacToeServer : MonoBehaviour {
        [SerializeField] private int _size = 3;
        [SerializeField] private TicTacToeInteractor _interactor;
        private Server _server;
        private Game _game;

        private void OnEnable() {
            Application.runInBackground = true;
            Debug.Log("Starting server");

            var server = new Server(44444, "a");
            server.OnClientConnected.Subscribe(OnClientConnected);

            if (!server.Start())
                Debug.LogWarning("Server start failure");
            else
                _server = server;
        }

        private void Update() {
            _server?.PollEvents();
        }

        private void OnDisable() {
            _server?.OnClientConnected.Unsubscribe(OnClientConnected);
            _server?.Dispose();
            _server = null;
        }

        private void OnClientConnected(IPeer client, bool connected) {
            if (!connected) {
                Debug.Log("Client disconnected");
                return;
            }

            Debug.Log("Client connected");

            _game = Game.StartServer(CreateConfigurationForUser(1), _server.ClientConnections.Peers, _interactor);

            _game.RegisterPresenter(_interactor);
            _interactor.SetCurrentUser(1);

            var winChecker = new TicTacToeWinChecker(_game.EventsAPI);
            _game.RegisterGenerator(winChecker);
            _game.RegisterPresenter(winChecker);

            client.Send(CreateConfigurationForUser(2), () => _game.Start());

            GameConfigurationMessage CreateConfigurationForUser(int id) {
                var users = new[] {1, 2};
                return new GameConfigurationMessage {
                    InitialCommand = new TicTacToeInitialCommand {
                        Size = _size,
                        Players = users,
                        Marks = new [] {MarkData.EMark.X, MarkData.EMark.O}
                    },
                    CurrenUser = id,
                    UserSequence = users,
                    UserNames = new []{ "server", "client"}
                };
            }
        }
    }
}
