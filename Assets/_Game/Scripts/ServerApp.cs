using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Network;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace _Game.Scripts {
    public class ServerApp : MonoBehaviour {
        private NetManager _server;
        private Coroutine _pollingCoro;

        private readonly Dictionary<NetPeer, Peer> _peers = new Dictionary<NetPeer, Peer>();

        private void OnEnable() {
            Debug.Log("Starting server");

            _server = CreateServer();
            if (!_server.Start(Constants.Port))
                Debug.LogWarning("Server start failure");

            _pollingCoro = StartCoroutine(PollEvents(_server));
        }

        private NetManager CreateServer() {
            var listener = new EventBasedNetListener();

            var deliveryEvent = new Event<NetPeer, object>(out var deliveryInvoker);
            listener.DeliveryEvent += (peer, data) => deliveryInvoker(peer, data);

            var networkReceiveEvent = new Event<NetPeer, NetDataReader>(out var networkReceiveInvoker);
            listener.NetworkReceiveEvent += (peer, reader, channel, method) => networkReceiveInvoker(peer, reader);

            listener.ConnectionRequestEvent += request => {
                Debug.Log($"Connection request: {request.RemoteEndPoint}");
                
                request.AcceptIfKey(Constants.AuthKey);
            };
            listener.PeerConnectedEvent += netPeer => {
                Debug.Log($"Peer connected: {netPeer.Id}, {netPeer.EndPoint}");

                var peer = new Peer(netPeer, deliveryEvent, networkReceiveEvent);
                peer.GetReceiveEvent<ChatMessage>().Subscribe(OnChatMessageReceive);
                peer.GetReceiveEvent<InitialInfoRequestMessage>().Subscribe(OnInitialInfoRequestMessageReceived);
                peer.Send(new ChatMessage {
                    Text = "WELL HELLO THERE"
                }, () => Debug.Log("Server chat message sent"));
                _peers.Add(netPeer, peer);
            };
            listener.PeerDisconnectedEvent += (netPeer, info) => {
                Debug.Log($"Peer disconnected: {netPeer.Id} {netPeer.EndPoint}, info: {info}");
                if (_peers.TryGetValue(netPeer, out var peer)) {
                    peer.Dispose();
                    _peers.Remove(netPeer);
                }
            };

            return new NetManager(listener);
        }

        private static IEnumerator PollEvents(NetManager pollingTarget) {
            while (true) {
                pollingTarget.PollEvents();
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }

        private void OnDisable() {
            StopCoroutine(_pollingCoro);
            _server.PollEvents();
            _server.Stop();
        }

        private void OnChatMessageReceive(ChatMessage message, Peer peer) {
            Debug.Log($"Chat message from client: {message.Text}");
        }

        private void OnInitialInfoRequestMessageReceived(InitialInfoRequestMessage message, Peer peer) {
            Debug.Log("Got initial info request");
            peer.Send(new InitialInfoMessage {
                Seed = 42
            });
        }
    }
}
