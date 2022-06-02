using System;
using System.Collections.Generic;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public class ECS : IComponentUpdateProxy {
        private readonly Func<int> _idCreator;
        private readonly Dictionary<int, Entity> _entities = new Dictionary<int, Entity>();
        public IReadOnlyCollection<Entity> Entities => _entities.Values;

        public ECS(Func<int> idCreator = null) {
            var startId = 0;
            int DefaultIdCreator() {
                return startId++;
            }

            _idCreator = idCreator ?? DefaultIdCreator;
        }

        public void AddEntity(Func<int, Entity> entityCreator) {
            var entity = entityCreator(_idCreator());
            foreach (var component in entity.Components) {
                component.SubscribeProxy(this);
            }

            _entities.Add(entity.Id, entity);
            // TODO entityCreated callback
        }

        public void RemoveEntity(int id) {
            if (!_entities.TryGetValue(id, out var entity)) {
                throw new ArgumentException($"No entity with id {id} found", nameof(id));
            }

            _entities.Remove(id);
            foreach (var component in entity.Components) {
                component.SubscribeProxy(this);
            }

            // TODO entityRemoved callback
        }

        public Event<TComponentData, IReadOnlyComponent<TComponentData>> GetComponentUpdateEvent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData> {
            throw new NotImplementedException();
        }

        public void RegisterComponent<TComponentData>(Type componentType, Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData> {
            throw new NotImplementedException();
        }

        public void UnregisterComponent<TComponentData>(Type componentType, Event<TComponentData, IReadOnlyComponent<TComponentData>> updateEvent)
            where TComponentData : struct, ISame<TComponentData> {
            throw new NotImplementedException();
        }
    }
}
