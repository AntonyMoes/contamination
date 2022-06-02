namespace _Game.Scripts.ModelV4.ECS {
    public interface IReadOnlyEntity {
        public int Id { get; }

        public IReadOnlyComponent<TComponentData> GetReadOnlyComponent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData>;
    }

    public interface IEntity : IReadOnlyEntity {
        public IComponent<TComponentData> GetModifiableComponent<TComponent, TComponentData>()
            where TComponent : Component<TComponentData>
            where TComponentData : struct, ISame<TComponentData>;
    }
}
