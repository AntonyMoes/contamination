using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.ModelV4 {
    public class GameDataReadAPI {
        private readonly ECS.ECS _ecs;
        public IReadOnlyCollection<IReadOnlyEntity> Entities => _ecs.Entities;

        public GameDataReadAPI(ECS.ECS ecs) {
            _ecs = ecs;
        }
    }
}
