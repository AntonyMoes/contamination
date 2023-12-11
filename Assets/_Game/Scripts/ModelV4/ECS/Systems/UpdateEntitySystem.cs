using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS.Systems {
    public abstract class UpdateEntitySystem : System {
        private readonly Proxy _proxy;

        protected virtual bool TriggerInitialUpdate => false;

        public UpdateEntitySystem() {
            _proxy = new Proxy(TrackedEntities, OnEntityUpdate);
        }

        protected override void OnEntityAdded(IEntity entity) {
            foreach (var component in GetComponents(entity)!) {
                component!.SubscribeProxy(_proxy, TriggerInitialUpdate);
            }
        }

        protected override void OnEntityRemoved(IEntity entity) {
            foreach (var component in GetComponents(entity)!) {
                component!.UnsubscribeProxy(_proxy);
            }
        }

        protected abstract void OnEntityUpdate(IEntity entity);

        private class Proxy : IComponentUpdateProxy {
            private readonly Dictionary<object, Action> _unsubscribers = new Dictionary<object, Action>();
            private readonly IReadOnlyCollection<IEntity> _trackedEntities;
            private readonly Action<IEntity> _onEntityUpdate;

            public Proxy(IReadOnlyCollection<IEntity> trackedEntities, Action<IEntity> onEntityUpdate) {
                _trackedEntities = trackedEntities;
                _onEntityUpdate = onEntityUpdate;
            }

            public void RegisterComponent<TComponentData>(Event<TComponentData?, IReadOnlyComponent<TComponentData>> updateEvent,
                IReadOnlyComponent<TComponentData> initialData)
                where TComponentData : struct, ISame<TComponentData> {
                updateEvent.Subscribe(Subscriber);
                _unsubscribers.Add(updateEvent, () => updateEvent.Unsubscribe(Subscriber));

                if (initialData != null) {
                    Subscriber(default, initialData);
                }

                void Subscriber(TComponentData? data, IReadOnlyComponent<TComponentData> component) {
                    _onEntityUpdate(_trackedEntities.First(e => e.Id == component.ReadOnlyEntity.Id));
                }
            }

            public void UnregisterComponent<TComponentData>(Event<TComponentData?, IReadOnlyComponent<TComponentData>> updateEvent)
                where TComponentData : struct, ISame<TComponentData> {
                _unsubscribers[updateEvent]();
            }
        }
    }
}