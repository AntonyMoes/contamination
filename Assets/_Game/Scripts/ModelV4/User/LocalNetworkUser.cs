using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4.User {
    public class LocalNetworkUser : IUser {
        private readonly LocalUser _localUser;
        private readonly INetworkCommandSender _sender;

        public int Id => _localUser.Id;
        public string Name => _localUser.Name;

        public LocalNetworkUser(LocalUser localUser, INetworkCommandSender sender) {
            _localUser = localUser;
            _sender = sender;
            
            OnCommandGenerated = new Event<GameCommand>(out var onCommandGenerated);
            _localUser.OnCommandGenerated.Subscribe(command => {
                _sender.SendCommand(Id, command);
                onCommandGenerated(command);
            });
        }

        public Event<GameCommand> OnCommandGenerated { get; }

        public void SetReadAPI(GameDataReadAPI api) => _localUser.SetReadAPI(api);
        public Event<bool> OnUserTurnToggled => _localUser.OnUserTurnToggled;

        public Process EndTurn() {
            var endTurnProcess = new SerialProcess();
            endTurnProcess.Add(_localUser.EndTurn());
            endTurnProcess.Add(_sender.SynchronizeSent(Id));
            return endTurnProcess;
        }
        public void StartTurn() {
            _localUser.StartTurn();
        }
    }
}
