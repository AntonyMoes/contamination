using System;
using System.Collections.Generic;
using GeneralUtils;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class Peer : IDisposable, IPeer {
        public readonly NetPeer NetPeer;

        private readonly NetPacketProcessor _processor;
        private readonly Event<NetPeer, object> _deliveryEvent;
        private readonly Event<NetPeer, NetDataReader> _networkReceiveEvent;

        private readonly Dictionary<Guid, Action> _deliveryCallbacks = new Dictionary<Guid, Action>();
        private readonly Dictionary<Type, object> _receiveEvents = new Dictionary<Type, object>();
        private readonly NetDataWriter _cachedWriter = new NetDataWriter();

        public Peer(NetPeer peer, Event<NetPeer, object> deliveryEvent, Event<NetPeer, NetDataReader> networkReceiveEvent) {
            NetPeer = peer;
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
                NetPeer.Send(_cachedWriter, deliveryMethod);
                return;
            }

            var guid = Guid.NewGuid(); 
            _deliveryCallbacks.Add(guid, onDone);
            NetPeer.SendWithDeliveryEvent(_cachedWriter, 0, deliveryMethod, guid);
        }

        private void OnDelivery(NetPeer peer, object data) {
            if (peer != NetPeer)
                return;

            var guid = (Guid) data;
            if (_deliveryCallbacks.TryGetValue(guid, out var callback))
                callback();
        }

        private void OnNetworkReceive(NetPeer peer, NetDataReader reader) {
            if (peer != NetPeer)
                return;

            _processor.ReadAllPackets(reader, this);
        }

        public Event<T, Peer> GetReceiveEvent<T>() where T : INetSerializable, new() {
            if (_receiveEvents.TryGetValue(typeof(T), out var eventObject))
                return (Event<T, Peer>) eventObject;

            var newEvent = new Event<T, Peer>(out var invoker);
            _processor.SubscribeNetSerializable(invoker, () => new T());
            _receiveEvents[typeof(T)] = newEvent;
            return newEvent;
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
