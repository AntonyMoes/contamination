using System.Collections.Generic;
using System.Linq;

namespace _Game.Scripts.ModelV3 {
    public class SimpleRunner {
        private ICommandCreator[] _creators;
        private ICommandReactor[] _reactors;

        public void SetCreators(IEnumerable<ICommandCreator> creators) {
            if (_creators != null) {
                foreach (var creator in _creators) {
                    creator.OnCommandCreated.Unsubscribe(OnCommandCreated);
                }
            }

            _creators = creators.ToArray();
            foreach (var creator in _creators) {
                creator.OnCommandCreated.Subscribe(OnCommandCreated);
            }
        }

        public void SetReactors(IEnumerable<ICommandReactor> reactors) {
            _reactors = reactors.ToArray();
        }

        private void OnCommandCreated(GameCommand command) {
            command.Do();
            foreach (var reactor in _reactors) {
                if (reactor.ShouldReactToCommand(command))
                    reactor.ReactToCommand(command);
            }
        }
    }
}
