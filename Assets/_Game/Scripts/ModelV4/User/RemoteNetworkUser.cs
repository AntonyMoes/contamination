using System;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4.User {
    public class RemoteNetworkUser : User {
        private readonly INetworkCommandReceiver _receiver;
        private readonly Action<GameCommand> _onCommandGenerated;

        public RemoteNetworkUser(int id, string name, INetworkCommandReceiver receiver) : base(id, name) {
            _receiver = receiver;
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            receiver.OnUserCommandReceived.Subscribe(OnRemoteUserCommandGenerated);
        }

        private void OnRemoteUserCommandGenerated(GameCommand command, int userId) {
            if (userId == Id)
                _onCommandGenerated(command);
        }

        public sealed override void SetReadAPI(GameDataReadAPI api) { }
        public override Event<GameCommand> OnCommandGenerated { get; }

        protected override Process PerformEndTurn() {
            return _receiver.SynchronizeReceived(Id);
        }

        public override void Dispose() {
            _receiver.OnUserCommandReceived.Unsubscribe(OnRemoteUserCommandGenerated);
        }
    }
}
