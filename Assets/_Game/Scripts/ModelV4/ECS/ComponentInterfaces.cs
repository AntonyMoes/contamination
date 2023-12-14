using GeneralUtils;

namespace _Game.Scripts.ModelV4.ECS {
    public interface IComponent {
        public void SubscribeProxy(IComponentUpdateProxy updateProxy, bool triggerInitialUpdate);
        public void UnsubscribeProxy(IComponentUpdateProxy updateProxy);
    }

    public interface IReadOnlyComponent<TComponentData> : IComponent
        where TComponentData : struct, ISame<TComponentData> {
        public IReadOnlyEntity ReadOnlyEntity { get; }
        public TComponentData Data { get; }
        public Event<TComponentData?, IReadOnlyComponent<TComponentData>> OnDataUpdate { get; }
    }

    public interface IComponent<TComponentData> : IReadOnlyComponent<TComponentData>
        where TComponentData : struct, ISame<TComponentData> {
        public IEntity Entity { get; }
        public new TComponentData Data { get; set; }
    }
}
