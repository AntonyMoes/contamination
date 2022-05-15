using System;
using System.Collections.Generic;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class Peer : IDisposable {
        private readonly NetPeer _peer;
        private readonly NetPacketProcessor _processor;
        private readonly Event<NetPeer, object> _deliveryEvent;
        private readonly Event<NetPeer, NetDataReader> _networkReceiveEvent;

        private readonly Dictionary<Guid, Action> _deliveryCallbacks = new Dictionary<Guid, Action>();
        private readonly Dictionary<Type, object> _receiveEvents = new Dictionary<Type, object>();
        private readonly NetDataWriter _cachedWriter = new NetDataWriter();

        public Peer(NetPeer peer, Event<NetPeer, object> deliveryEvent, Event<NetPeer, NetDataReader> networkReceiveEvent) {
            _peer = peer;
            _processor = new NetPacketProcessor();
            _deliveryEvent = deliveryEvent;
            _deliveryEvent.Subscribe(OnDelivery);
            _networkReceiveEvent = networkReceiveEvent;
            _networkReceiveEvent.Subscribe(OnNetworkReceive);
        }

        public void Send<T>(T data, Action onDone = null) where T : class, new() {
            const DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered;

            _cachedWriter.Reset();
            _processor.Write(_cachedWriter, data);

            if (onDone == null) {
                _peer.Send(_cachedWriter, deliveryMethod);
                return;
            }

            var guid = Guid.NewGuid(); 
            _deliveryCallbacks.Add(guid, onDone);
            _peer.SendWithDeliveryEvent(_cachedWriter, 0, deliveryMethod, guid);
        }

        private void OnDelivery(NetPeer peer, object data) {
            if (peer != _peer)
                return;

            var guid = (Guid) data;
            if (_deliveryCallbacks.TryGetValue(guid, out var callback))
                callback();
        }

        private void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            if (peer != _peer)
                return;
            
            _processor.ReadAllPackets(reader, this);
        }

        public Event<T, Peer> GetReceiveEvent<T>() where T : class, new() {
            if (_receiveEvents.TryGetValue(typeof(T), out var eventObject))
                return (Event<T, Peer>) eventObject;

            var newEvent = new Event<T, Peer>(out var invoker);
            _processor.Subscribe(invoker, () => new T());
            _receiveEvents[typeof(T)] = newEvent;
            return newEvent;
        }

        public void Dispose() {
            _deliveryEvent.Unsubscribe(OnDelivery);
            _networkReceiveEvent.Unsubscribe(OnNetworkReceive);
        }
    }
}
