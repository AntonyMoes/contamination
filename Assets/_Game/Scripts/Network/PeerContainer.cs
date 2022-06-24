using System;
using System.Linq;
using GeneralUtils;
using GeneralUtils.Processes;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public class PeerContainer : IPeer {
        private readonly PeerCollection _collection = new PeerCollection();

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

        public void SendMessage<TMessage>(TMessage message, Message respondsTo = null, Action onDone = null)
            where TMessage : Message, new() {
            _collection.SendMessage(message, respondsTo, null, onDone);
        }

        public Process SendWithResponse<TMessage, TResponse>(TMessage message, Message respondsTo = null, Action<TResponse, IPeer> onDone = null)
            where TMessage : Message, new()
            where TResponse : Message, new() {
            return _collection.SendWithResponse<TMessage, TResponse>(message, respondsTo, null, (messages, peers) => {
                if (messages.Length != 0) {
                    onDone?.Invoke(messages[0], peers[0]);
                }
            });
        }

        public Event<T, IPeer> GetReceiveEvent<T>() where T : INetSerializable, new() {
            return _collection.GetReceiveEvent<T>();
        }

        public void Dispose() {
            _collection.Dispose();
        }
    }
}
