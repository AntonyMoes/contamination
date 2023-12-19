using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game;
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
            var startProcess = new ParallelProcess();
            _game = GameStarter.StartServerGame(settings, users, QueueMessage);
            _game.EventsAPI.OnGameEnded.Subscribe(OnGameEnded);
            startProcess.Run(_game.Start);

            void QueueMessage(LobbyUser user, GameConfigurationMessage message) {
                var peer = users.First(u => u.Id == user.Id).Peer;
                startProcess.Add(AsyncProcess.From(peer.Send, message));
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
