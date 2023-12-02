using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.User;

namespace _Game.Scripts.ModelV4 {
    public class GameDataReadAPI : IGameReadAPI {
        private readonly ModelV4.ECS.ECS _ecs;
        private readonly TurnController _turnController;

        public IReadOnlyCollection<IReadOnlyEntity> Entities => _ecs.Entities;
        public IReadOnlyCollection<IReadOnlyUser> UserSequence => _turnController.UserSequence;
        public IReadOnlyUser CurrentUser => _turnController.CurrentUser;

        public GameDataReadAPI(ModelV4.ECS.ECS ecs, TurnController turnController) {
            _ecs = ecs;
            _turnController = turnController;
        }
    }
}
