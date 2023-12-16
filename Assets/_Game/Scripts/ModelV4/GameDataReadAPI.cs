using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.User;
using _Game.Scripts.Utils;

namespace _Game.Scripts.ModelV4 {
    public class GameDataReadAPI : IGameReadAPI {
        private readonly ModelV4.ECS.ECS _ecs;
        private readonly ProxyDictionary<int, IReadOnlyEntity, Entity> _proxyEntities;
        private readonly TurnController _turnController;

        public IReadOnlyDictionary<int, IReadOnlyEntity> Entities => _proxyEntities;
        public IReadOnlyCollection<IReadOnlyUser> UserSequence => _turnController.UserSequence;
        public IReadOnlyUser CurrentUser => _turnController.CurrentUser;
        public int CurrentTurn => _turnController.CurrentTurn;

        public GameDataReadAPI(ModelV4.ECS.ECS ecs, TurnController turnController) {
            _ecs = ecs;
            _proxyEntities = new ProxyDictionary<int, IReadOnlyEntity, Entity>(_ecs.Entities);
            _turnController = turnController;
        }
    }
}
