using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.Lobby;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel.Network;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Network {
    public class StandaloneLobbyServer : MonoBehaviour {
        private Server _server;
        private ModelV4.Game _game;
        private Lobby<RoomSettings> _lobby;

        private void OnEnable() {
            Application.runInBackground = true;
            Debug.Log("Starting server");
            
            var server = new Server(44444, "a");
            server.OnClientConnected.Subscribe(OnClientConnected);
            _lobby = new Lobby<RoomSettings>(server.ClientConnections, StartGame);

            if (!server.Start())
                Debug.LogWarning("Server start failure");
            else
                _server = server;
        }

        private void StartGame(RoomSettings settings, IReadOnlyCollection<LobbyUser> users) {
            var orderedMarks = new[] { MarkData.EMark.X, MarkData.EMark.O };
            var orderedUsers = settings.Users.ToArray();

            var startProcess = new ParallelProcess();
            for (var i = 0; i < RoomSettings.MaxUsers; i++) {
                var user = orderedUsers[i];
                var peer = users.First(u => u.Id == user.Id).Peer;
                startProcess.Add(AsyncProcess.From(peer.Send, CreateConfigurationForUser(IdFromIndex(i))));
            }

            _game = ModelV4.Game.StartStandaloneServer(CreateConfigurationForUser(), users.Select(u => u.Peer));

            var winChecker = new TicTacToeWinChecker(_game.EventsAPI);
            _game.RegisterGenerator(winChecker);
            _game.RegisterPresenter(winChecker);

            _game.EventsAPI.OnGameEnded.Subscribe(OnGameEnded);

            startProcess.Run(_game.Start);

            static int IdFromIndex(int index) => index + 1;
            GameConfigurationMessage CreateConfigurationForUser(int id = 0) {
                var userIds = Enumerable.Range(0, RoomSettings.MaxUsers).Select(IdFromIndex).ToArray();
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
