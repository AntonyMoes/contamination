using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using GeneralUtils.Processes;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class PeerCollection : IDisposable {
        private readonly bool _owner;
        private readonly List<Peer> _peers = new List<Peer>();
        public IReadOnlyList<Peer> Peers => _peers;

        private readonly Dictionary<Type, object> _receiveEvents = new Dictionary<Type, object>();
        private readonly List<Action<Peer>> _receiveSubscribers = new List<Action<Peer>>();
        private readonly List<Action<Peer>> _receiveUnsubscribers = new List<Action<Peer>>();

        public PeerCollection(bool owner = true) {
            _owner = owner;
        }

        public void Add(Peer peer) {
            if (_peers.Contains(peer))
                return;

            _peers.Add(peer);
            foreach (var subscriber in _receiveSubscribers)
                subscriber(peer);
        }

        public void Remove(Peer peer, bool dispose = true) {
            if (!_peers.Remove(peer))
                return;

            foreach (var unsubscriber in _receiveUnsubscribers)
                unsubscriber(peer);

            if (dispose)
                peer.Dispose();
        }

        public Peer GetByNetPeer(NetPeer netPeer) {
            return _peers.FirstOrDefault(peer => peer.NetPeer == netPeer);
        }

        public void Send<T>(T data, Peer except = null, Action onDone = null) where T : INetSerializable, new() {
            var sendProcess = new ParallelProcess();
            foreach (var peer in _peers) {
                if (peer == except)
                    continue;

                sendProcess.Add(AsyncProcess.From(peer.Send, data));
            }

            sendProcess.Run(onDone);
        }

        public Event<T, Peer> GetReceiveEvent<T>() where T : INetSerializable, new() {
            if (_receiveEvents.TryGetValue(typeof(T), out var eventObject))
                return (Event<T, Peer>) eventObject;

            var newEvent = new Event<T, Peer>(out var invoker);
            _receiveEvents[typeof(T)] = newEvent;
            Action<Peer> subscriber = peer => peer.GetReceiveEvent<T>().Subscribe(invoker);
            _receiveSubscribers.Add(subscriber);
            Action<Peer> unsubscriber = peer => peer.GetReceiveEvent<T>().Unsubscribe(invoker);
            _receiveUnsubscribers.Add(unsubscriber);

            foreach (var peer in _peers)
                subscriber(peer);

            return newEvent;
        }

        public void Dispose() {
            foreach (var peer in _peers) {
                foreach (var unsubscriber in _receiveUnsubscribers)
                    unsubscriber(peer);

                if (_owner)
                    peer.Dispose();
            }
        }
    }
}
