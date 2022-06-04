using System;
using System.Collections.Generic;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public class ECS : IComponentUpdateProxy {
        private readonly Func<int> _idCreator;
        private readonly Dictionary<Type, object> _componentUpdateEvents = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _componentUpdateInvokers = new Dictionary<Type, object>();

        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        public IReadOnlyCollection<Entity> Entities => _entities.Values;

        private readonly Action<Entity> _onEntityCreated;
        public readonly Event<Entity> OnEntityCreated;
        private readonly Action<Entity> _onEntityDestroyed;
        public readonly Event<Entity> OnEntityDestroyed;

        public ECS(Func<int> idCreator = null) {
            var startId = 0;
            int DefaultIdCreator() {
                return startId++;
            }

            _idCreator = idCreator ?? DefaultIdCreator;

            OnEntityCreated = new Event<Entity>(out _onEntityCreated);
            OnEntityDestroyed = new Event<Entity>(out _onEntityDestroyed);
        }

        public void AddEntity(Func<int, Entity> entityCreator) {
            var entity = entityCreator(_idCreator());
            foreach (var component in entity.Components) {
                component.SubscribeProxy(this);
            }

            _entities.Add(entity.Id, entity);
            _onEntityCreated(entity);
        }

        public void RemoveEntity(int id) {
            if (!_entities.TryGetValue(id, out var entity)) {
                throw new ArgumentException($"No entity with id {id} found", nameof(id));
            }

            _entities.Remove(id);
            foreach (var component in entity.Components) {
                component.SubscribeProxy(this);
            }

            _onEntityDestroyed(entity);
        }

        public Event<TComponentData, IReadOnlyComponent<TComponentData>> GetComponentUpdateEvent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            return GetEventPair<TComponentData>(typeof(TComponent)).updateEvent;
        }

        public void RegisterComponent<TComponentData>(Type componentType, Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData> {
            updateEvent.Subscribe(GetEventPair<TComponentData>(componentType).invoker);
        }

        public void UnregisterComponent<TComponentData>(Type componentType, Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData> {
            updateEvent.Unsubscribe(GetEventPair<TComponentData>(componentType).invoker);
        }

        private (Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent, Action<TComponentData, IReadOnlyComponent<TComponentData>> invoker)
            GetEventPair<TComponentData>(Type componentType)
            where TComponentData : struct, ISame<TComponentData> {
            if (_componentUpdateEvents.TryGetValue(componentType, out var updateEventObject)) {
                var invokerObject = _componentUpdateInvokers[componentType];
                return ((Event<TComponentData, IReadOnlyComponent<TComponentData>>) updateEventObject,
                    (Action<TComponentData, IReadOnlyComponent<TComponentData>>) invokerObject);
            }

            var updateEvent = new Event<TComponentData, IReadOnlyComponent<TComponentData>>(out var invoker);
            _componentUpdateEvents[componentType] = updateEvent;
            _componentUpdateInvokers[componentType] = invoker;
            return (updateEvent, invoker);
        }
    }
}
