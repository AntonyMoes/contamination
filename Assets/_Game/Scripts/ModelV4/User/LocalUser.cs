using System;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4.User {
    public class LocalUser : User {
        private readonly ICommandGenerator _userCommandGenerator;

        public LocalUser(int id, string name, ICommandGenerator userCommandGenerator) : base (id, name){
            _userCommandGenerator = userCommandGenerator;
        }

        public sealed override void SetReadAPI(GameDataReadAPI api) => _userCommandGenerator.SetReadAPI(api);
        public sealed override Event<GameCommand> OnCommandGenerated => _userCommandGenerator.OnCommandGenerated;

        protected override Process PerformEndTurn() => new DummyProcess();
    }
}
