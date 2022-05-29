using System.Collections.Generic;
using GeneralUtils;

namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class ProxyCreator : ICommandCreator {
        private bool _enabled = true;
        public ProxyCreator(TicTacToeState state, IEnumerable<ICommandCreator> creators) {
            OnCommandCreated = new Event<GameCommand>(out var onCommandCreated);
            foreach (var creator in creators) {
                creator.OnCommandCreated.Subscribe(Wrapper);
            }

            void Wrapper(GameCommand command) {
                if (!_enabled) {
                    return;
                }

                if (command is MarkCommand markCommand)
                    markCommand.SetState(state);

                onCommandCreated(command);
            }
        }

        public void Disable() {
            _enabled = false;
        }

        public Event<GameCommand> OnCommandCreated { get; }
    }
}
