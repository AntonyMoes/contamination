using GeneralUtils;

namespace _Game.Scripts.ModelV4 {
    public interface ICommandSource {
        public Event<GameCommand> OnCommandGenerated { get; }
    }
}
