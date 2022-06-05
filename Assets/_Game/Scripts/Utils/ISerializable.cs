namespace _Game.Scripts.Utils {
    public interface ISerializable {
        public string SerializeContents();
        public void DeserializeContents(string contents);
    }
}
