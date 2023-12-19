using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Systems;

namespace _Game.Scripts.BurnMark.Game {
    public static class GameMechanicsRegistry {
        public static void RegisterMechanics(ModelV4.Game game, FieldAccessor fieldAccessor) {
            game.RegisterSystem(new ResourceGainSystem());
            game.RegisterSystem(new UnitBuildingSystem(fieldAccessor));
            game.RegisterSystem(new ActionsResetSystem());
            game.RegisterSystem(new DeathSystem());
            game.RegisterGenerator(new GameEndChecker(game.EventsAPI));
        }
    }
}