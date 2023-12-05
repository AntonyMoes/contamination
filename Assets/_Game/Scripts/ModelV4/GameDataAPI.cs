using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using JetBrains.Annotations;

namespace _Game.Scripts.ModelV4 {
    public class GameDataAPI : GameDataReadAPI, IGameAPI {
        private readonly ModelV4.ECS.ECS _ecs;
        private readonly TurnController _turnController;

        public IReadOnlyCollection<IEntity> ModifiableEntities => _ecs.Entities;

        public GameDataAPI(ModelV4.ECS.ECS ecs, TurnController turnController) : base(ecs, turnController) {
            _ecs = ecs;
            _turnController = turnController;
        }

        public int AddEntity(Func<int, Entity> entityCreator) {
            return _ecs.AddEntity(entityCreator);
        }

        public void RemoveEntity(int id) {
            _ecs.RemoveEntity(id);
        }

        [CanBeNull]
        public IEntity GetModifiableEntity(int id) {
            return _ecs.GetEntity(id);
        }

        public void EndTurn(bool endGame = false) {
            _turnController.EndTurn(endGame);
        }
    }
}
