
namespace _Game.Scripts.Network {
    public class ChatMessage {
        public string Text { get; set; }
    }

    public class InitialInfoRequestMessage { }

    public class InitialInfoMessage {
        public int Seed { get; set; }
    }
}
