using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.ModelV4 {
    public class GameDataAPI {
        private readonly ECS.ECS _ecs;
        public IReadOnlyCollection<IEntity> Entities => _ecs.Entities;

        public GameDataAPI(ECS.ECS ecs) {
            _ecs = ecs;
        }

        public void AddEntity(Func<int, Entity> entityCreator) {
            _ecs.AddEntity(entityCreator);
        }

        public void RemoveEntity(int id) {
            _ecs.RemoveEntity(id);
        }
    }
}
