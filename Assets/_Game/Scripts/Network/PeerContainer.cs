using System;
using System.Linq;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class PeerContainer : IPeer {
        private readonly PeerCollection _collection = new PeerCollection();

        public PeerContainer() { }

        public IPeer SetPeer(IPeer peer = null) {
            IPeer previousPeer = null;
            if (_collection.Peers.Count > 0) {  // == 1
                previousPeer = _collection.Peers.First();
                _collection.Remove(previousPeer);
            }

            if (peer != null) {
                _collection.Add(peer);
            }

            return previousPeer;
        }

        public bool CorrespondsTo(NetPeer netPeer) {
            return _collection.Peers.All(peer => peer.CorrespondsTo(netPeer));
        }

        public void Send<T>(T data, Action onDone = null) where T : INetSerializable, new() {
            _collection.Send(data, null, onDone);
        }

        public Event<T, IPeer> GetReceiveEvent<T>() where T : INetSerializable, new() {
            return _collection.GetReceiveEvent<T>();
        }

        public void Dispose() {
            _collection.Dispose();
        }
    }
}
