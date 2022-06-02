using System;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public class Component<TComponentData> : IComponent, IComponent<TComponentData>
        where TComponentData : struct, ISame<TComponentData> {
        private TComponentData _data;

        public Entity Entity { get; }

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

        private readonly Action<TComponentData, IReadOnlyComponent<TComponentData>> _onDataUpdateInvoker;
        private readonly Event<TComponentData, IReadOnlyComponent<TComponentData>> _onDataUpdate;

        public Component() {
            _onDataUpdate = new Event<TComponentData, IReadOnlyComponent<TComponentData>>(out _onDataUpdateInvoker);
        }

        public void SubscribeProxy(IComponentUpdateProxy updateProxy) {
            updateProxy.RegisterComponent(GetType(), _onDataUpdate);
        }

        public void UnsubscribeProxy(IComponentUpdateProxy updateProxy) {
            updateProxy.UnregisterComponent(GetType(), _onDataUpdate);
        }
    }
}
