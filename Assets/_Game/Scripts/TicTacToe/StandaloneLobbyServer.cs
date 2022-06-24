using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4;
using _Game.Scripts.Network;
using UnityEngine;
using _Game.Scripts.Lobby;
using _Game.Scripts.ModelV4.Network;
using _Game.Scripts.TicTacToe.Commands;
using GeneralUtils.Processes;
using LiteNetLib.Utils;

namespace _Game.Scripts.TicTacToe {
    public class StandaloneLobbyServer : MonoBehaviour {
        [SerializeField] private TicTacToeInteractor _interactor;
        private Server _server;
        private Game _game;
        private Lobby<TicTacToeRoomSettings> _lobby;

        private void OnEnable() {
            Application.runInBackground = true;
            Debug.Log("Starting server");
            
            var server = new Server(44444, "a");
            server.OnClientConnected.Subscribe(OnClientConnected);
            _lobby = new Lobby<TicTacToeRoomSettings>(server.ClientConnections, StartGame);

            if (!server.Start())
                Debug.LogWarning("Server start failure");
            else
                _server = server;
        }

        private void StartGame(TicTacToeRoomSettings settings, IReadOnlyCollection<LobbyUser> users) {
            _game = Game.StartStandaloneServer(CreateConfigurationForUser(), _server.ClientConnections.Peers);

            _game.RegisterPresenter(_interactor);
            _interactor.SetCurrentUser(1);

            var winChecker = new TicTacToeWinChecker(_game.EventsAPI);
            _game.RegisterGenerator(winChecker);
            _game.RegisterPresenter(winChecker);

            var startProcess = new ParallelProcess();
            for (var i = 0; i < TicTacToeRoomSettings.MaxUsers; i++) {
                var user = settings.Users[i];
                var peer = users.First(u => u.Id == user.Id).Peer;
                startProcess.Add(AsyncProcess.From(peer.Send, CreateConfigurationForUser(IdFromIndex(i))));
            }
            startProcess.Run(_game.Start);

            static int IdFromIndex(int index) => index + 1;
            GameConfigurationMessage CreateConfigurationForUser(int id = 0) {
                var users = Enumerable.Range(0, TicTacToeRoomSettings.MaxUsers).Select(IdFromIndex).ToArray();
                return new GameConfigurationMessage {
                    InitialCommand = new TicTacToeInitialCommand {
                        Size = 3,
                        Players = users,
                        Marks = settings.Marks
                    },
                    CurrenUser = id,
                    UserSequence = users,
                    UserNames = settings.Users.Select(u => u.Name).ToArray()
                };
            }
        }

        private void Update() {
            _server?.PollEvents();
        }

        private void OnDisable() {
            _lobby.Dispose();
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
        }
    }
}
