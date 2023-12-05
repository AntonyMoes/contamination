using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel.User {
    public interface INetworkCommandReceiver {
        public Event<GameCommand, int> OnUserCommandReceived { get; }
        public Process SynchronizeReceived(int userId);
    }
}
