using _Game.Scripts.Network;
using UnityEngine;

namespace _Game.Scripts.NetworkingExample {
    public class ClientApp : MonoBehaviour {
        private Client _client;

        private void OnEnable() {
            _client = new Client(Constants.Host, Constants.Port, Constants.AuthKey);
            _client.ServerConnection.GetReceiveEvent<ChatMessage>().Subscribe(OnChatMessageReceive);
            _client.ServerConnection.GetReceiveEvent<InitialInfoMessage>().Subscribe(OnInitialInfoMessageReceived);
            _client.OnConnected.Subscribe(OnConnected);

            _client.Start();
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

            _client.ServerConnection.Send(new InitialInfoRequestMessage());
        }

        private void OnChatMessageReceive(ChatMessage message, IPeer peer) {
            Debug.Log($"Chat message from server: {message.Text}");
            peer.Send(new ChatMessage {
                Text = "well hello indeed"
            });
        }

        private void OnInitialInfoMessageReceived(InitialInfoMessage message, IPeer peer) {
            Debug.Log($"Received initial info: seed={message.Seed}, list={string.Join(",", message.List)}");
        }
    }
}
