using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Lobby;
using _Game.Scripts.ModelV4;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Network;
using _Game.Scripts.TicTacToe.Commands;
using _Game.Scripts.TicTacToe.Data;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;

// using LiteNetLib.Utils;

namespace _Game.Scripts.TicTacToe {
    public class StandaloneLobbyServer : MonoBehaviour {
        // [SerializeField] private TicTacToeInteractor _interactor;
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
            var orderedMarks = new[] { MarkData.EMark.X, MarkData.EMark.O };
            var orderedUsers = settings.Users.OrderBy(u => orderedMarks.IndexOf(settings.Marks[settings.Users.IndexOf(u)])).ToArray();

            var startProcess = new ParallelProcess();
            for (var i = 0; i < TicTacToeRoomSettings.MaxUsers; i++) {
                var user = orderedUsers[i];
                var peer = users.First(u => u.Id == user.Id).Peer;
                startProcess.Add(AsyncProcess.From(peer.Send, CreateConfigurationForUser(IdFromIndex(i))));
            }

            _game = Game.StartStandaloneServer(CreateConfigurationForUser(), users.Select(u => u.Peer));

            var winChecker = new TicTacToeWinChecker(_game.EventsAPI);
            _game.RegisterGenerator(winChecker);
            _game.RegisterPresenter(winChecker);

            _game.EventsAPI.OnGameEnded.Subscribe(OnGameEnded);

            startProcess.Run(_game.Start);

            static int IdFromIndex(int index) => index + 1;
            GameConfigurationMessage CreateConfigurationForUser(int id = 0) {
                var userIds = Enumerable.Range(0, TicTacToeRoomSettings.MaxUsers).Select(IdFromIndex).ToArray();
                return new GameConfigurationMessage {
                    InitialCommand = new TicTacToeInitialCommand {
                        Size = 3,
                        Players = userIds,
                        Marks = orderedMarks
                    },
                    CurrenUser = id,
                    UserSequence = userIds,
                    UserNames = orderedUsers.Select(u => u.Name).ToArray(),
                };
            }
        }

        private void OnGameEnded() {
            _game.EventsAPI.OnGameEnded.Unsubscribe(OnGameEnded);
            _game.Dispose();
            _game = null;
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
