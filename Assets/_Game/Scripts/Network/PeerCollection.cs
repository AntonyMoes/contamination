using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using GeneralUtils.Processes;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class PeerCollection : IDisposable, IPeerCollection {
        private readonly bool _owner;
        private readonly List<IPeer> _peers = new List<IPeer>();
        public IReadOnlyList<IPeer> Peers => _peers;

        private readonly Dictionary<Type, object> _receiveEvents = new Dictionary<Type, object>();
        private readonly List<Action<IPeer>> _receiveSubscribers = new List<Action<IPeer>>();
        private readonly List<Action<IPeer>> _receiveUnsubscribers = new List<Action<IPeer>>();

        public PeerCollection(bool owner = true) {
            _owner = owner;
        }

        public void Add(IPeer peer) {
            if (_peers.Contains(peer))
                return;

            _peers.Add(peer);
            foreach (var subscriber in _receiveSubscribers)
                subscriber(peer);
        }

        public void Remove(IPeer peer) {
            if (!_peers.Remove(peer))
                return;

            foreach (var unsubscriber in _receiveUnsubscribers)
                unsubscriber(peer);

            if (_owner)
                peer.Dispose();
        }

        public IPeer GetByNetPeer(NetPeer netPeer) {
            return _peers.FirstOrDefault(peer => peer.CorrespondsTo(netPeer));
        }

        public void Send<T>(T data, INetworkSender except = null, Action onDone = null) where T : INetSerializable, new() {
            var sendProcess = new ParallelProcess();
            foreach (var peer in _peers) {
                if (peer == except)
                    continue;

                sendProcess.Add(AsyncProcess.From(peer.Send, data));
            }

            sendProcess.Run(onDone);
        }

        public Event<T, IPeer> GetReceiveEvent<T>() where T : INetSerializable, new() {
            return (Event<T, IPeer>) _receiveEvents.GetValue(typeof(T), Initializer);

            Event<T, IPeer> Initializer() {
                var newEvent = new Event<T, IPeer>(out var invoker);
                Action<IPeer> subscriber = peer => peer.GetReceiveEvent<T>().Subscribe(invoker);
                _receiveSubscribers.Add(subscriber);
                Action<IPeer> unsubscriber = peer => peer.GetReceiveEvent<T>().Unsubscribe(invoker);
                _receiveUnsubscribers.Add(unsubscriber);

                foreach (var peer in _peers)
                    subscriber(peer);

                return newEvent;
            }
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
