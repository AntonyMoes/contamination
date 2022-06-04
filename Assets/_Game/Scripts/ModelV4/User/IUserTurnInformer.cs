using GeneralUtils;

namespace _Game.Scripts.ModelV4.User {
    public interface IUserTurnInformer {
        public Event<bool> OnUserTurnToggled { get; }
    }
}
