using GeneralUtils;

namespace _Game.Scripts.NetworkModel.User {
    public interface IUserTurnInformer {
        public Event<bool> OnUserTurnToggled { get; }
    }
}
