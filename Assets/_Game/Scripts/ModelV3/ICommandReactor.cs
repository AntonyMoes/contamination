namespace _Game.Scripts.ModelV3 {
    public interface ICommandReactor {
        public bool ShouldReactToCommand(GameCommand command);
        public void ReactToCommand(GameCommand command);
    }
}
