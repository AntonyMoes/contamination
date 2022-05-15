using System.Collections;
using _Game.Scripts.Network;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace _Game.Scripts {
    public class ClientApp : MonoBehaviour{
        private NetManager _client;
        private Coroutine _pollingCoro;
        private Peer _serverPeer;

        private void OnEnable() {
            Debug.Log("Starting client");
            var listener = new EventBasedNetListener();

            var deliveryEvent = new Event<NetPeer, object>(out var deliveryInvoker);
            listener.DeliveryEvent += (peer, data) => deliveryInvoker(peer, data);

            var networkReceiveEvent = new Event<NetPeer, NetDataReader>(out var networkReceiveInvoker);
            listener.NetworkReceiveEvent += (peer, reader, channel, method) => networkReceiveInvoker(peer, reader);

            listener.PeerConnectedEvent += netPeer => {
                Debug.Log($"Connected to server: {netPeer.Id}, {netPeer.EndPoint}");
                _serverPeer = new Peer(netPeer, deliveryEvent, networkReceiveEvent);
                _serverPeer.GetReceiveEvent<ChatMessage>().Subscribe(OnChatMessageReceive);
                _serverPeer.GetReceiveEvent<InitialInfoMessage>().Subscribe(OnInitialInfoMessageReceived);
                _serverPeer.Send(new InitialInfoRequestMessage());
            };

            _client = new NetManager(listener);
            _client.Start();
            _client.Connect(Network.Constants.Host, Network.Constants.Port, Network.Constants.AuthKey);

            _pollingCoro = StartCoroutine(PollEvents(_client));
        }

        private static IEnumerator PollEvents(NetManager pollingTarget) {
            while (true) {
                pollingTarget.PollEvents();
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }

        private void OnDisable() {
            StopCoroutine(_pollingCoro);
            _client.PollEvents();
            _serverPeer.Dispose();
            _client.Stop();
        }

        private void OnChatMessageReceive(ChatMessage message, Peer peer) {
            Debug.Log($"Chat message from server: {message.Text}");
            peer.Send(new ChatMessage {
                Text = "well hello indeed"
            });
        }

        private void OnInitialInfoMessageReceived(InitialInfoMessage message, Peer peer) {
            Debug.Log($"Received initial info: seed={message.Seed}");
        }
    }
}
