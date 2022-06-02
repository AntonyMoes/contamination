namespace _Game.Scripts.ModelV4.ECS {
    public interface ISame<T> {
        public bool IsSame(ISame<T> other);
    }
}
