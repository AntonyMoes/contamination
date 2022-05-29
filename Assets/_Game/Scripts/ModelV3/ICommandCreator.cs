using GeneralUtils;

namespace _Game.Scripts.ModelV3 {
    public interface ICommandCreator {
        public Event<GameCommand> OnCommandCreated { get; }
    }
}
