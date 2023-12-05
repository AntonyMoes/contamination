using System;
using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel.User {
    public class LocalUser : User {
        private readonly ICommandGenerator _userCommandGenerator;
        private readonly Action<GameCommand> _onCommandGenerated;

        public LocalUser(int id, string name, ICommandGenerator userCommandGenerator) : base (id, name){
            _userCommandGenerator = userCommandGenerator;
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            _userCommandGenerator.OnCommandGenerated.Subscribe(OnLocalCommandGenerated);
        }

        private void OnLocalCommandGenerated(GameCommand command) {
            _onCommandGenerated(command);
        }

        public sealed override void SetReadAPI(IGameReadAPI api) => _userCommandGenerator.SetReadAPI(api);
        public sealed override Event<GameCommand> OnCommandGenerated { get; }

        protected override Process PerformEndTurn() => new DummyProcess();

        public override void Dispose() {
            _userCommandGenerator.OnCommandGenerated.Unsubscribe(OnLocalCommandGenerated);
        }
    }
}
