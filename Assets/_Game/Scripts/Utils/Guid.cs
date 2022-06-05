namespace _Game.Scripts.Utils {
    public static class Guid {
        public static string New => System.Guid.NewGuid().ToString();
    }
}
