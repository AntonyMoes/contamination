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

        // private readonly Dictionary<NetPeer, Peer> _peers = new Dictionary<NetPeer, Peer>();
        private PeerCollection _peerCollection;

        private void OnEnable() {
            Debug.Log("Starting server");

            _server = CreateServer();
            if (!_server.Start(Constants.Port))
                Debug.LogWarning("Server start failure");

            _pollingCoro = StartCoroutine(PollEvents(_server));
        }

        private NetManager CreateServer() {
            var listener = new EventBasedNetListener();
            var peerInitializer = Peer.CreateInitializerForEventListener(listener);
            _peerCollection = new PeerCollection();
            _peerCollection.GetReceiveEvent<ChatMessage>().Subscribe(OnChatMessageReceive);
            _peerCollection.GetReceiveEvent<InitialInfoRequestMessage>().Subscribe(OnInitialInfoRequestMessageReceived);

            listener.ConnectionRequestEvent += request => {
                Debug.Log($"Connection request: {request.RemoteEndPoint}");

                request.AcceptIfKey(Constants.AuthKey);
            };
            listener.PeerConnectedEvent += netPeer => {
                Debug.Log($"Peer connected: {netPeer.Id}, {netPeer.EndPoint}");

                var peer = peerInitializer(netPeer);
                _peerCollection.Add(peer);
                peer.Send(new ChatMessage {
                    Text = "WELL HELLO THERE"
                }, () => Debug.Log("Server chat message sent"));
            };
            listener.PeerDisconnectedEvent += (netPeer, info) => {
                Debug.Log($"Peer disconnected: {netPeer.Id} {netPeer.EndPoint}, info: {info}");

                if (_peerCollection.GetByNetPeer(netPeer) is {} peer)
                    _peerCollection.Remove(peer);
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
            _peerCollection.Dispose();
            _server.Stop();
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
