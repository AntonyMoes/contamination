namespace _Game.Scripts.ModelV4.ECS {
    public interface ISame<in T> {
        public bool IsSame(T other);
    }

    public static class SameHelper {
        // public static T Get<T>(this ISame<T> same) => (T) same;
    }
}
