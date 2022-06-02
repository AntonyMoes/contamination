namespace _Game.Scripts.ModelV4.ECS {
    public interface IComponent {
        public void SubscribeProxy(IComponentUpdateProxy updateProxy);
        public void UnsubscribeProxy(IComponentUpdateProxy updateProxy);
    }

    public interface IReadOnlyComponent<out TComponentData> : IComponent
        where TComponentData : struct {
        public Entity Entity { get; }
        public TComponentData Data { get; }
    }

    public interface IComponent<TComponentData> : IReadOnlyComponent<TComponentData>
        where TComponentData : struct {
        public new TComponentData Data { get; set; }
    }
}
