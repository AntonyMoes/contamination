using _Game.Scripts.BurnMark.Game.Systems;

namespace _Game.Scripts.BurnMark.Game {
    public static class GameMechanicsRegistry {
        public static void RegisterMechanics(ModelV4.Game game) {
            var resourceGainSystem = new ResourceGainSystem();
            game.RegisterSystem(resourceGainSystem);
        }
    }
}