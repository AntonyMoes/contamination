using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.User;

namespace _Game.Scripts.ModelV4 {
    public class GameDataReadAPI {
        private readonly ECS.ECS _ecs;
        private readonly TurnController _turnController;

        public IReadOnlyCollection<IReadOnlyEntity> Entities => _ecs.Entities;
        public IReadOnlyCollection<IReadOnlyUser> UserSequence => _turnController.UserSequence;
        public IReadOnlyUser CurrentUser => _turnController.CurrentUser;

        public GameDataReadAPI(ECS.ECS ecs, TurnController turnController) {
            _ecs = ecs;
            _turnController = turnController;
        }
    }
}
