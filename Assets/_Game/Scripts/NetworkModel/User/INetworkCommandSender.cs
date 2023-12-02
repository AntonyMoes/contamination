using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel.User {
    public interface INetworkCommandSender {
        public void SendCommand(int userId, GameCommand command);
        public Process SynchronizeSent(int userId);
    }
}
