using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4.User {
    public class RemoteNetworkUser : User {
        private readonly INetworkCommandReceiver _receiver;

        public RemoteNetworkUser(int id, string name, INetworkCommandReceiver receiver) : base(id, name) {
            _receiver = receiver;
            OnCommandGenerated = new Event<GameCommand>(out var onCommandGenerated);
            receiver.OnUserCommandReceived.Subscribe((command, userId) => {
                if (userId == id)
                    onCommandGenerated(command);
            });
        }

        public sealed override void SetReadAPI(GameDataReadAPI api) { }
        public override Event<GameCommand> OnCommandGenerated { get; }

        protected override Process PerformEndTurn() {
            return _receiver.SynchronizeReceived(Id);
        }
    }
}
