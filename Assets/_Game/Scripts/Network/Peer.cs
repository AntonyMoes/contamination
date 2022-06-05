using System;
using System.Collections.Generic;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class Peer : IPeer {
        private readonly NetPeer _netPeer;

        private readonly NetPacketProcessor _processor;
        private readonly Event<NetPeer, object> _deliveryEvent;
        private readonly Event<NetPeer, NetDataReader> _networkReceiveEvent;

        private readonly Dictionary<Guid, Action> _deliveryCallbacks = new Dictionary<Guid, Action>();
        private readonly Dictionary<Type, object> _receiveEvents = new Dictionary<Type, object>();
        private readonly NetDataWriter _cachedWriter = new NetDataWriter();

        public Peer(NetPeer peer, Event<NetPeer, object> deliveryEvent, Event<NetPeer, NetDataReader> networkReceiveEvent) {
            _netPeer = peer;
            _processor = new NetPacketProcessor();
            _deliveryEvent = deliveryEvent;
            _deliveryEvent.Subscribe(OnDelivery);
            _networkReceiveEvent = networkReceiveEvent;
            _networkReceiveEvent.Subscribe(OnNetworkReceive);
        }

        public void Send<T>(T data, Action onDone = null) where T : INetSerializable, new() { // TODO: add timeouts?
            const DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered;

            _cachedWriter.Reset();
            _processor.WriteNetSerializable(_cachedWriter, ref data);

            if (onDone == null) {
                _netPeer.Send(_cachedWriter, deliveryMethod);
                return;
            }

            var guid = Guid.NewGuid(); 
            _deliveryCallbacks.Add(guid, onDone);
            _netPeer.SendWithDeliveryEvent(_cachedWriter, 0, deliveryMethod, guid);
        }

        public bool CorrespondsTo(NetPeer netPeer) {
            return _netPeer == netPeer;
        }

        private void OnDelivery(NetPeer peer, object data) {
            if (peer != _netPeer)
                return;

            var guid = (Guid) data;
            if (_deliveryCallbacks.TryGetValue(guid, out var callback))
                callback();
            _deliveryCallbacks.Remove(guid);
        }

        private void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            if (peer != _netPeer)
                return;

            _processor.ReadAllPackets(reader, this);
        }

        public Event<T, IPeer> GetReceiveEvent<T>() where T : INetSerializable, new() {
            return (Event<T, IPeer>) _receiveEvents.GetValue(typeof(T), Initializer);

            Event<T, IPeer> Initializer() {
                var newEvent = new Event<T, IPeer>(out var invoker);
                _processor.SubscribeNetSerializable(invoker, () => new T());
                return newEvent;
            }
        }

        public void Dispose() {
            _deliveryEvent.Unsubscribe(OnDelivery);
            _networkReceiveEvent.Unsubscribe(OnNetworkReceive);
        }

        public static Func<NetPeer, Peer> CreateInitializerForEventListener(EventBasedNetListener listener) {
            var deliveryEvent = new Event<NetPeer, object>(out var deliveryInvoker);
            listener.DeliveryEvent += (peer, data) => deliveryInvoker(peer, data);

            var networkReceiveEvent = new Event<NetPeer, NetDataReader>(out var networkReceiveInvoker);
            listener.NetworkReceiveEvent += (peer, reader, channel, method) => {
                networkReceiveInvoker(peer, reader);
            };

            return netPeer => new Peer(netPeer, deliveryEvent, networkReceiveEvent);
        }
    }
}
