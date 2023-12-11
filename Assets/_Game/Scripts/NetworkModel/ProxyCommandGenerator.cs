using System;
using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;

namespace _Game.Scripts.NetworkModel {
    public class ProxyCommandGenerator : ICommandGenerator {
        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        public ProxyCommandGenerator() {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
        }
        public void SetReadAPI(IGameReadAPI api) { }

        public void GenerateCommand(GameCommand command) => _onCommandGenerated(command);
    }
}