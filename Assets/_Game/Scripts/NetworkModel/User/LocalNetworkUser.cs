using System;
using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel.User {
    public class LocalNetworkUser : IUser {
        private readonly LocalUser _localUser;
        private readonly INetworkCommandSender _sender;
        private readonly Action<GameCommand> _onCommandGenerated;

        public int Id => _localUser.Id;
        public string Name => _localUser.Name;

        public LocalNetworkUser(LocalUser localUser, INetworkCommandSender sender) {
            _localUser = localUser;
            _sender = sender;
            
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            _localUser.OnCommandGenerated.Subscribe(OnLocalUserCommandGenerated);
        }

        private void OnLocalUserCommandGenerated(GameCommand command) {
            _sender.SendCommand(Id, command);
            _onCommandGenerated(command);
        }

        public Event<GameCommand> OnCommandGenerated { get; }

        public void SetReadAPI(IGameReadAPI api) => _localUser.SetReadAPI(api);
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

        public void Dispose() {
            _localUser.OnCommandGenerated.Unsubscribe(OnLocalUserCommandGenerated);
        }
    }
}
