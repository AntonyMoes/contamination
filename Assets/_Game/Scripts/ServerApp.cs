using System.Collections.Generic;
using _Game.Scripts.Network;
using UnityEngine;

namespace _Game.Scripts {
    public class ServerApp : MonoBehaviour {
        private Server _server;

        private void OnEnable() {
            Debug.Log("Starting server");

            var server = new Server(Constants.Port, Constants.AuthKey);
            server.ClientConnections.GetReceiveEvent<ChatMessage>().Subscribe(OnChatMessageReceive);
            server.ClientConnections.GetReceiveEvent<InitialInfoRequestMessage>().Subscribe(OnInitialInfoRequestMessageReceived);
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
            client.Send(new ChatMessage {
                Text = "WELL HELLO THERE"
            }, () => Debug.Log("Server chat message sent"));
        }

        private void OnChatMessageReceive(ChatMessage message, Peer peer) {
            Debug.Log($"Chat message from client: {message.Text}");
        }

        private void OnInitialInfoRequestMessageReceived(InitialInfoRequestMessage message, Peer peer) {
            Debug.Log("Got initial info request");
            peer.Send(new InitialInfoMessage {
                Seed = 42,
                List = new List<int> {1, 2}
            });
        }
    }
}
