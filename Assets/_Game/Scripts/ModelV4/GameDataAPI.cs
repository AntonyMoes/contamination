using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.Utils;

namespace _Game.Scripts.ModelV4 {
    public class GameDataAPI : GameDataReadAPI, IGameAPI {
        private readonly ModelV4.ECS.ECS _ecs;
        private readonly ProxyDictionary<int, IEntity, Entity> _proxyEntities;
        private readonly TurnController _turnController;

        public IReadOnlyDictionary<int, IEntity> ModifiableEntities => _proxyEntities;

        public GameDataAPI(ModelV4.ECS.ECS ecs, TurnController turnController) : base(ecs, turnController) {
            _ecs = ecs;
            _proxyEntities = new ProxyDictionary<int, IEntity, Entity>(_ecs.Entities);
            _turnController = turnController;
        }

        public int AddEntity(Func<int, Entity> entityCreator) {
            return _ecs.AddEntity(entityCreator);
        }

        public void RemoveEntity(int id) {
            _ecs.RemoveEntity(id);
        }

        public void EndTurn(bool endGame = false) {
            _turnController.EndTurn(endGame);
        }
    }
}
