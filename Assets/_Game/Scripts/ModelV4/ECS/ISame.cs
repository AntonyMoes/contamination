namespace _Game.Scripts.ModelV4.ECS {
    public interface ISame<T> {
        public bool IsSame(ISame<T> other);
    }

    public static class SameHelper {
        public static T Get<T>(this ISame<T> same) => (T) same;
    }
}
