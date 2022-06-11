namespace _Game.Scripts.ModelV4.ECS {
    public interface IReadOnlyEntity {
        public int Id { get; }

        public IReadOnlyComponent<TComponentData> GetReadOnlyComponent<TComponentData>()
            where TComponentData : struct, ISame<TComponentData>;
    }

    public interface IEntity : IReadOnlyEntity {
        public IComponent<TComponentData> GetModifiableComponent<TComponentData>()
            where TComponentData : struct, ISame<TComponentData>;
    }
}
