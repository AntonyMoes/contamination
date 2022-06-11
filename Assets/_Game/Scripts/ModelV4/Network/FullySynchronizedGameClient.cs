using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.User;
using _Game.Scripts.Network;
using GeneralUtils;
using GeneralUtils.Processes;
using Guid = _Game.Scripts.Utils.Guid;

namespace _Game.Scripts.ModelV4.Network {
    // A client that waits for all other clients to receive its updates.
    // I will write the corresponding server if this will actually be used.
    public class FullySynchronizedGameClient : INetworkCommandSender, INetworkCommandReceiver {
        private readonly IPeer _serverPeer;
        private readonly ValueWaiter<int> _notSynchronizedMessages = new ValueWaiter<int>();
        private readonly List<string> _notSynchronizedGuids = new List<string>();
        private readonly Action<GameCommand, int> _onUserCommandReceived;
        private readonly Dictionary<int, ValueWaiter<string>> _synchronizationFinishers =
            new Dictionary<int, ValueWaiter<string>>();

        public FullySynchronizedGameClient(IPeer serverPeer) {
            _serverPeer = serverPeer;
            _serverPeer.GetReceiveEvent<SynchronizeMessage>().Subscribe(OnMessageSynchronized);
            _serverPeer.GetReceiveEvent<GameCommandMessage>().Subscribe(OnGameCommandReceived);
            _serverPeer.GetReceiveEvent<FinishSynchronizationMessage>().Subscribe(OnSynchronizationFinished);

            OnUserCommandReceived = new Event<GameCommand, int>(out _onUserCommandReceived);
        }

        private ValueWaiter<string> GetSynchronizationFinisher(int userId) =>
            _synchronizationFinishers.GetValue(userId, () => new ValueWaiter<string>(null));

        private void OnSynchronizationFinished(FinishSynchronizationMessage message, IPeer _) {
            var finisher = GetSynchronizationFinisher(message.UserId);
            finisher.Value = message.Guid;
        }

        private void OnGameCommandReceived(GameCommandMessage message, IPeer _) {
            _onUserCommandReceived(message.Command, message.UserId);
        }

        private void AddNotSynchronized(string guid) {
            _notSynchronizedGuids.Add(guid);
            _notSynchronizedMessages.Value++;
        }

        private void OnMessageSynchronized(SynchronizeMessage message, IPeer _) {
            if (!_notSynchronizedGuids.Contains(message.Guid)) 
                throw new ArgumentException($"Received an attempt to synchronize an unknown Guid: {message.Guid}", nameof(message.Guid));

            _notSynchronizedGuids.Remove(message.Guid);
            _notSynchronizedMessages.Value--;
        }

        public void SendCommand(int userId, GameCommand command) {
            var guid = Guid.New;
            AddNotSynchronized(guid);
            _serverPeer.Send(new GameCommandMessage {
                Guid = guid,
                UserId = userId,
                Command = command
            });
        }
        public Process SynchronizeSent(int userId) {
            var guid = Guid.New;
            AddNotSynchronized(guid);
            _serverPeer.Send(new FinishSynchronizationMessage {
                Guid = guid,
                UserId = userId
            });
            return AsyncProcess.From(_notSynchronizedMessages.WaitFor, 0);
        }

        public Event<GameCommand, int> OnUserCommandReceived { get; }
        public Process SynchronizeReceived(int userId) {
            var finisher = GetSynchronizationFinisher(userId);
            static bool IsSynchronized(string value) => value != null;

            var synchronizationProcess = new SerialProcess();
            synchronizationProcess.Add(new AsyncProcess(onDone => finisher.WaitFor(IsSynchronized, onDone)));
            synchronizationProcess.Add(new SyncProcess(() => finisher.Value = null));
            return synchronizationProcess;
        }
    }
}
