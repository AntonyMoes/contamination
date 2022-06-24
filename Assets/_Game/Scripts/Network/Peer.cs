using System;
using System.Collections.Generic;
using GeneralUtils;
using GeneralUtils.Processes;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

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

        public void SendMessage<TMessage>(TMessage message, Message respondsTo = null, Action onDone = null)
            where TMessage : Message, new() {
            message.SetRespondsToId(respondsTo?.Id);
            Send(message, onDone);
        }

        public Process SendWithResponse<TMessage, TResponse>(TMessage message, Message respondsTo = null, Action<TResponse, IPeer> onDone = null)
            where TMessage : Message, new()
            where TResponse : Message, new() {
            var responseWaiter = new ValueWaiter<TResponse>();
            GetReceiveEvent<TResponse>().Subscribe(Subscriber);

            var trace = Environment.StackTrace;
            
            var sendProcess = new SerialProcess();
            Debug.Log($"Sending message of type {typeof(TMessage)} and awaiting response of type {typeof(TResponse)}");
            sendProcess.Add(SyncProcess.From(SendMessage, message, respondsTo, (Action) null));
            sendProcess.Add(new AsyncProcess(responseWaiter.WaitForChange));
            sendProcess.Add(new SyncProcess(() => {
                var a = trace;
                Debug.Log($"Invoking response handler of type {typeof(TResponse)} for message of type {typeof(TMessage)}");
                onDone?.Invoke(responseWaiter.Value, this);
            }));
            sendProcess.Run();

            return sendProcess;

            void Subscriber(TResponse response, IPeer _) {
                Debug.Log("Subscriber got response");
                if (response.RespondsToId == message.Id) {
                    GetReceiveEvent<TResponse>().Unsubscribe(Subscriber);
                    responseWaiter.Value = response;
                }
            }
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
    }
}
