using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4.User {
    public interface INetworkCommandReceiver {
        public Event<GameCommand, int> OnUserCommandReceived { get; }
        public Process SynchronizeReceived(int userId);
    }
}
