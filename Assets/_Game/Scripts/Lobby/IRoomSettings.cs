using LiteNetLib.Utils;

namespace _Game.Scripts.Lobby {
    public interface IRoomSettings<in TImplementation> : INetSerializable {
        public abstract bool CanJoin(LobbyUser.Data user);
        public abstract void Join(LobbyUser.Data user);
        public abstract void Leave(string userId);

        public abstract bool CanMakeUpdate(string userId, TImplementation update);
        public abstract void MakeUpdate(TImplementation update);

        public abstract bool CanStartGame(string userId);
    }
}
