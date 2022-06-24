using System;
using System.Net;
using System.Net.Sockets;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class Server : IDisposable, INetEventListener, IDeliveryEventListener {
        private readonly int _port;
        private readonly string _authKey;
        private readonly NetManager _server;
        private readonly PeerCollection _clientConnections;
        public IPeerCollection ClientConnections => _clientConnections;

        private readonly Action<NetPeer, NetDataReader> _onNetworkReceiveInvoker;
        private readonly Event<NetPeer, NetDataReader> _onNetworkReceive;
        private readonly Action<NetPeer, object> _onMessageDeliveredInvoker;
        private readonly Event<NetPeer, object> _onMessageDelivered;

        private readonly Action<IPeer, bool> _onClientConnectedInvoker;
        public readonly Event<IPeer, bool> OnClientConnected;

        public Server(int port, string authKey) {
            _port = port;
            _authKey = authKey;
            _server = new NetManager(this);

            _clientConnections = new PeerCollection();

            OnClientConnected = new Event<IPeer, bool>(out _onClientConnectedInvoker);
            _onNetworkReceive = new Event<NetPeer, NetDataReader>(out _onNetworkReceiveInvoker);
            _onMessageDelivered = new Event<NetPeer, object>(out _onMessageDeliveredInvoker);
        }

        public bool Start() {
            return _server.Start(_port);
        }

        public void PollEvents() {
            _server.PollEvents();
        }

        public void Dispose() {
            _clientConnections.Dispose();
            _server.Stop();
        }

        public void OnPeerConnected(NetPeer peer) {
            var client = new Peer(peer, _onMessageDelivered, _onNetworkReceive);
            _clientConnections.Add(client);
            _onClientConnectedInvoker(client, true);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
            if (!(_clientConnections.GetByNetPeer(peer) is { } client))
                throw new ArgumentException("WTF");

            _onClientConnectedInvoker(client, false);
            _clientConnections.Remove(client);
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {
            _onNetworkReceiveInvoker(peer, reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnConnectionRequest(ConnectionRequest request) {
            request.AcceptIfKey(_authKey);
        }

        public void OnMessageDelivered(NetPeer peer, object userData) {
            _onMessageDeliveredInvoker(peer, userData);
        }
    }
}
