﻿using System;
using System.Collections.Generic;
using GeneralUtils;
using LiteNetLib.Utils;

namespace _Game.Scripts.Network {
    public interface INetworkSender {
        public void Send<T>(T data, Action onDone = null) where T : INetSerializable, new();
    }

    public interface INetworkMultiSender {
        public void Send<T>(T data, INetworkSender except = null, Action onDone = null) where T : INetSerializable, new();
    }

    public interface INetworkReceiver {
        public Event<T, Peer> GetReceiveEvent<T>() where T : INetSerializable, new();
    }
    
    public interface IPeer : INetworkSender, INetworkReceiver { }

    public interface IPeerCollection : INetworkMultiSender, INetworkReceiver {
        public IReadOnlyList<IPeer> Peers { get; }
    }
}
