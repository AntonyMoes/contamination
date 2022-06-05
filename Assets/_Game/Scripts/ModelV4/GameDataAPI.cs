using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.ModelV4 {
    public class GameDataAPI {
        private readonly ECS.ECS _ecs;
        private readonly TurnController _turnController;

        public IReadOnlyCollection<IEntity> Entities => _ecs.Entities;

        public GameDataAPI(ECS.ECS ecs, TurnController turnController) {
            _ecs = ecs;
            _turnController = turnController;
        }

        public void AddEntity(Func<int, Entity> entityCreator) {
            _ecs.AddEntity(entityCreator);
        }

        public void RemoveEntity(int id) {
            _ecs.RemoveEntity(id);
        }

        public void EndTurn(bool endGame = false) {
            _turnController.EndTurn(endGame);
        }

        public void UndoEndTurn(bool gameEnded = false) {
            _turnController.UndoEndTurn(gameEnded);
        }
    }
}
