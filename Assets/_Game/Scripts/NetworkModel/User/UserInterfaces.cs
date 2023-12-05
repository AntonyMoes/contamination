using System;
using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel.User {
    public interface IReadOnlyUser : IUserTurnInformer {
        public int Id { get; }
        public string Name { get; }
    }

    public interface IUser : IReadOnlyUser, ICommandGenerator, IDisposable {
        public Process EndTurn();
        public void StartTurn();
    }
}
