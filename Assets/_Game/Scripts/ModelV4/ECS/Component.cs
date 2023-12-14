using System;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public class Component<TComponentData> : IComponent<TComponentData>
        where TComponentData : struct, ISame<TComponentData> {
        private TComponentData _data;

        public IReadOnlyEntity ReadOnlyEntity => Entity;
        public IEntity Entity { get; }

        public TComponentData Data {
            get => _data;
            set {
                if (_data.IsSame(value))
                    return;

                var oldData = _data;
                _data = value;
                _onDataUpdateInvoker(oldData, this);
            }
        }

        private readonly Action<TComponentData?, IReadOnlyComponent<TComponentData>> _onDataUpdateInvoker;
        public Event<TComponentData?, IReadOnlyComponent<TComponentData>> OnDataUpdate { get; }

        public Component(Entity correspondingEntity) {
            Entity = correspondingEntity;
            OnDataUpdate = new Event<TComponentData?, IReadOnlyComponent<TComponentData>>(out _onDataUpdateInvoker);
        }

        public void SubscribeProxy(IComponentUpdateProxy updateProxy, bool triggerInitialUpdate) {
            updateProxy.RegisterComponent(OnDataUpdate, triggerInitialUpdate ? this : null);
        }

        public void UnsubscribeProxy(IComponentUpdateProxy updateProxy) {
            updateProxy.UnregisterComponent(OnDataUpdate);
        }
    }
}
