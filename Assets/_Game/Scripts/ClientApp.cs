using _Game.Scripts.Network;
using UnityEngine;

namespace _Game.Scripts {
    public class ClientApp : MonoBehaviour{
        private Client _client;

        private void OnEnable() {
            _client = new Client(Constants.Host, Constants.Port, Constants.AuthKey);
            _client.Start();
            _client.OnConnected.Subscribe(OnConnected);
        }

        private void Update() {
            _client?.PollEvents();
        }

        private void OnDisable() {
            _client.Dispose();
            _client = null;
        }

        private void OnConnected(bool connected) {
            if (!connected) {
                Debug.Log("Server disconnected, shutting down");
                gameObject.SetActive(false);
                return;
            }

            _client.ServerConnection.GetReceiveEvent<ChatMessage>().Subscribe(OnChatMessageReceive);
            _client.ServerConnection.GetReceiveEvent<InitialInfoMessage>().Subscribe(OnInitialInfoMessageReceived);
            _client.ServerConnection.Send(new InitialInfoRequestMessage());
        }

        private void OnChatMessageReceive(ChatMessage message, Peer peer) {
            Debug.Log($"Chat message from server: {message.Text}");
            peer.Send(new ChatMessage {
                Text = "well hello indeed"
            });
        }

        private void OnInitialInfoMessageReceived(InitialInfoMessage message, Peer peer) {
            Debug.Log($"Received initial info: seed={message.Seed}, list={string.Join(",", message.List)}");
        }
    }
}
