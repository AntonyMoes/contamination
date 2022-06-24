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

        private Action<IPeer, bool> _peerConnectionEvent;
        public Event<IPeer, bool> OnPeerConnection { get; }

        private readonly Dictionary<Type, object> _receiveEvents = new Dictionary<Type, object>();
        private readonly List<Action<IPeer>> _receiveSubscribers = new List<Action<IPeer>>();
        private readonly List<Action<IPeer>> _receiveUnsubscribers = new List<Action<IPeer>>();

        public PeerCollection(bool owner = true) {
            _owner = owner;
            OnPeerConnection = new Event<IPeer, bool>(out _peerConnectionEvent);
        }

        public void Add(IPeer peer) {
            if (_peers.Contains(peer))
                return;

            _peers.Add(peer);
            foreach (var subscriber in _receiveSubscribers)
                subscriber(peer);

            _peerConnectionEvent(peer, true);
        }

        public void Remove(IPeer peer) {
            if (!_peers.Remove(peer))
                return;

            foreach (var unsubscriber in _receiveUnsubscribers)
                unsubscriber(peer);

            if (_owner)
                peer.Dispose();

            _peerConnectionEvent(peer, false);
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

        public void SendMessage<TMessage>(TMessage message, Message respondsTo = null, INetworkSender except = null,
            Action onDone = null) where TMessage : Message, new() {
            var sendProcess = new ParallelProcess();
            foreach (var peer in _peers) {
                if (peer == except)
                    continue;

                sendProcess.Add(AsyncProcess.From(peer.SendMessage, message, respondsTo));
            }

            sendProcess.Run(onDone);
        }

        public Process SendWithResponse<TMessage, TResponse>(TMessage message, Message respondsTo = null, INetworkSender except = null,
            Action<TResponse[], IPeer[]> onDone = null) where TMessage : Message, new() where TResponse : Message, new() {
            var responses = new TResponse[_peers.Count];
            var sendProcess = new ParallelProcess();
            for (var i = 0; i < _peers.Count; i++) {
                var peer = _peers[i];
                if (peer == except)
                    continue;

                var idx = i;
                sendProcess.Add(new AsyncProcess(callback => peer.SendWithResponse<TMessage, TResponse>
                (message, respondsTo, (response, _) => {
                    responses[idx] = response;
                    callback();
                })));
            }

            sendProcess.Run(() => {
                onDone?.Invoke(responses, _peers.ToArray());
            });

            return sendProcess;
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
