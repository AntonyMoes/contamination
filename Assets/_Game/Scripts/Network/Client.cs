using System;
using System.Net;
using System.Net.Sockets;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class Client : IDisposable, INetEventListener, IDeliveryEventListener {
        private readonly string _host;
        private readonly int _port;
        private readonly string _authKey;
        private readonly NetManager _client;
        private readonly PeerContainer _serverConnection  = new PeerContainer();
        public IPeer ServerConnection => _serverConnection;

        private readonly Action<NetPeer, NetDataReader> _onNetworkReceiveInvoker;
        private readonly Event<NetPeer, NetDataReader> _onNetworkReceive;
        private readonly Action<NetPeer, object> _onMessageDeliveredInvoker;
        private readonly Event<NetPeer, object> _onMessageDelivered;

        private readonly Action<bool> _onConnectedInvoker;
        public readonly Event<bool> OnConnected;

        public bool IsConnected => _serverConnection != null;

        public Client(string host, int port, string authKey) {
            _host = host;
            _port = port;
            _authKey = authKey;
            _client = new NetManager(this);

            OnConnected = new Event<bool>(out _onConnectedInvoker);
            _onNetworkReceive = new Event<NetPeer, NetDataReader>(out _onNetworkReceiveInvoker);
            _onMessageDelivered = new Event<NetPeer, object>(out _onMessageDeliveredInvoker);
        }

        public bool Start() {
            if (!_client.Start())
                return false;

            _client.Connect(_host, _port, _authKey);
            return true;
        }

        public void PollEvents() {
            _client.PollEvents();
        }

        public void Dispose() {
            _serverConnection?.Dispose();
            _client.Stop();
        }

        public void OnPeerConnected(NetPeer peer) {
            var previousPeer = _serverConnection.SetPeer(new Peer(peer, _onMessageDelivered, _onNetworkReceive));
            previousPeer?.Dispose();
            _onConnectedInvoker(true);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            // TODO handle initial connection errors
            if (!_serverConnection.CorrespondsTo(peer))
                throw new ArgumentException("WTF");

            var previousPeer = _serverConnection.SetPeer();
            previousPeer?.Dispose();
            _onConnectedInvoker(false);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {
            _onNetworkReceiveInvoker(peer, reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnConnectionRequest(ConnectionRequest request) { }

        public void OnMessageDelivered(NetPeer peer, object userData) {
            _onMessageDeliveredInvoker(peer, userData);
        }
    }
}
