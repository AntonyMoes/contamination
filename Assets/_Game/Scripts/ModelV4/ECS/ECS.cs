using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public class ECS : IComponentUpdateProxy {
        private readonly Func<int> _idCreator;
        private readonly Dictionary<Type, object> _componentUpdateEvents = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _componentUpdateInvokers = new Dictionary<Type, object>();

        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        public IReadOnlyDictionary<int, Entity> Entities => _entities;

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

        public int AddEntity(Func<int, Entity> entityCreator) {
            var entity = entityCreator(_idCreator());
            foreach (var component in entity.Components) {
                component.SubscribeProxy(this, true);
            }

            _entities.Add(entity.Id, entity);
            _onEntityCreated(entity);

            return entity.Id;
        }

        public void RemoveEntity(int id) {
            if (!_entities.TryGetValue(id, out var entity)) {
                throw new ArgumentException($"No entity with id {id} found", nameof(id));
            }

            _entities.Remove(id);
            foreach (var component in entity.Components) {
                component.UnsubscribeProxy(this);
            }

            _onEntityDestroyed(entity);
        }

        public void Clear() {
            foreach (var entityId in _entities.Keys.ToArray()) {
                RemoveEntity(entityId);
            }
        }

        public Event<TComponentData?, IReadOnlyComponent<TComponentData>> GetComponentUpdateEvent<TComponentData>()
            where TComponentData : struct, ISame<TComponentData> {
            return GetEventPair<TComponentData>(typeof(TComponentData)).updateEvent;
        }

        public void RegisterComponent<TComponentData>(Event<TComponentData?, IReadOnlyComponent<TComponentData>> updateEvent,
            IReadOnlyComponent<TComponentData> initialData)
            where TComponentData : struct, ISame<TComponentData> {
            updateEvent.Subscribe(GetEventPair<TComponentData>(typeof(TComponentData)).invoker);

            if (initialData != null) {
                GetEventPair<TComponentData>(typeof(TComponentData)).invoker(default, initialData);
            }
        }

        public void UnregisterComponent<TComponentData>(Event<TComponentData?, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData> {
            updateEvent.Unsubscribe(GetEventPair<TComponentData>(typeof(TComponentData)).invoker);
        }

        private (Event<TComponentData?, IReadOnlyComponent<TComponentData>> updateEvent, Action<TComponentData?, IReadOnlyComponent<TComponentData>> invoker)
            GetEventPair<TComponentData>(Type dataType)
            where TComponentData : struct, ISame<TComponentData> {
            if (_componentUpdateEvents.TryGetValue(dataType, out var updateEventObject)) {
                var invokerObject = _componentUpdateInvokers[dataType];
                return ((Event<TComponentData?, IReadOnlyComponent<TComponentData>>) updateEventObject,
                    (Action<TComponentData?, IReadOnlyComponent<TComponentData>>) invokerObject);
            }

            var updateEvent = new Event<TComponentData?, IReadOnlyComponent<TComponentData>>(out var invoker);
            _componentUpdateEvents[dataType] = updateEvent;
            _componentUpdateInvokers[dataType] = invoker;
            return (updateEvent, invoker);
        }
    }
}
