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
        private readonly Event<TComponentData?, IReadOnlyComponent<TComponentData>> _onDataUpdate;

        public Component(Entity correspondingEntity) {
            Entity = correspondingEntity;
            _onDataUpdate = new Event<TComponentData?, IReadOnlyComponent<TComponentData>>(out _onDataUpdateInvoker);
        }

        public void SubscribeProxy(IComponentUpdateProxy updateProxy, bool triggerInitialUpdate) {
            updateProxy.RegisterComponent(_onDataUpdate, triggerInitialUpdate ? this : null);
        }

        public void UnsubscribeProxy(IComponentUpdateProxy updateProxy) {
            updateProxy.UnregisterComponent(_onDataUpdate);
        }
    }
}
