using System;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;

namespace _Game.Scripts.ModelV4 {
    public class GameDataEventsAPI {
        private readonly ECS.ECS _ecs;

        public GameDataEventsAPI(ECS.ECS ecs) {
            _ecs = ecs;
        }

        public Event<TComponentData, IReadOnlyComponent<TComponentData>> GetComponentUpdateEvent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            return _ecs.GetComponentUpdateEvent<TComponent, TComponentData>();
        }
    }
}
