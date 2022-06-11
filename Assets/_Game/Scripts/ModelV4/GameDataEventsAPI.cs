using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.User;
using GeneralUtils;

namespace _Game.Scripts.ModelV4 {
    public class GameDataEventsAPI {
        private readonly ECS.ECS _ecs;
        private readonly TurnController _turnController;

        public Event<Entity> OnEntityCreated => _ecs.OnEntityCreated;
        public Event<Entity> OnEntityDestroyed => _ecs.OnEntityDestroyed;

        public Event<IReadOnlyUser, IReadOnlyUser> OnTurnChanged => _turnController.OnTurnChanged;
        public readonly Event OnGameEnded;

        public GameDataEventsAPI(ECS.ECS ecs, TurnController turnController) {
            _ecs = ecs;
            _turnController = turnController;
            OnGameEnded = new Event(out var onGameEnded);
            _turnController.OnTurnChanged.Subscribe((user1, user2) => {
                if (user2 == null) {
                    onGameEnded();
                }
            });
        }

        public Event<TComponentData, IReadOnlyComponent<TComponentData>> GetComponentUpdateEvent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            return _ecs.GetComponentUpdateEvent<TComponent, TComponentData>();
        }
    }
}
