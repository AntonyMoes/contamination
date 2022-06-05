using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.User;
using _Game.Scripts.Network;
using GeneralUtils;
using GeneralUtils.Processes;
using Guid = _Game.Scripts.Utils.Guid;

namespace _Game.Scripts.ModelV4.Network {
    public class GameClient : INetworkCommandSender, INetworkCommandReceiver {
        private readonly Peer _serverPeer;
        private readonly ValueWaiter<int> _notSynchronizedMessages = new ValueWaiter<int>();
        private readonly Action<GameCommand, int> _onUserCommandReceived;
        private readonly Dictionary<int, ValueWaiter<string>> _synchronizationFinishers =
            new Dictionary<int, ValueWaiter<string>>();

        public GameClient(Peer serverPeer) {
            _serverPeer = serverPeer;
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

        public void SendCommand(int userId, GameCommand command) {
            _notSynchronizedMessages.Value++;
            _serverPeer.Send(new GameCommandMessage {
                Guid = Guid.New,
                UserId = userId,
                Command = command
            }, () => {
                _notSynchronizedMessages.Value--;
            });
        }
        public Process SynchronizeSent(int userId) {
            _notSynchronizedMessages.Value++;
            _serverPeer.Send(new FinishSynchronizationMessage {
                Guid = Guid.New,
                UserId = userId
            }, () => {
                _notSynchronizedMessages.Value--;
            });
            return new AsyncProcess(onDone => _notSynchronizedMessages.WaitFor(0, onDone));
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
