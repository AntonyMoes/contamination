using System;
using System.Collections.Generic;
using GeneralUtils;
using GeneralUtils.Processes;
using LiteNetLib;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public interface INetworkSender {
        public void Send<T>(T data, Action onDone = null) where T : INetSerializable, new();

        public void SendMessage<TMessage>(TMessage message, Message respondsTo = null, Action onDone = null)
            where TMessage : Message, new();

        public Process SendWithResponse<TMessage, TResponse>(TMessage message, Message respondsTo = null, Action<TResponse, IPeer> onDone = null)
            where TMessage : Message, new()
            where TResponse : Message, new();
    }

    public interface INetworkMultiSender {
        public void Send<T>(T data, INetworkSender except = null, Action onDone = null) where T : INetSerializable, new();

        public void SendMessage<TMessage>(TMessage message, Message respondsTo = null, INetworkSender except = null, Action onDone = null)
            where TMessage : Message, new();

        public Process SendWithResponse<TMessage, TResponse>(TMessage message, Message respondsTo = null, INetworkSender except = null, Action<TResponse[], IPeer[]> onDone = null)
            where TMessage : Message, new()
            where TResponse : Message, new();
    }

    public interface INetworkReceiver {
        public Event<T, IPeer> GetReceiveEvent<T>() where T : INetSerializable, new();
    }

    public interface IPeer : INetworkSender, INetworkReceiver, IDisposable {
        bool CorrespondsTo(NetPeer netPeer);
    }

    public interface IPeerCollection : INetworkMultiSender, INetworkReceiver {
        public IReadOnlyList<IPeer> Peers { get; }
        public Event<IPeer, bool> OnPeerConnection { get; }
    }
}
