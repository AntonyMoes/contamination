using System;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4.User {
    public interface IReadOnlyUser {
        public int Id { get; }
        public string Name { get; }
    }

    public interface IUser : IReadOnlyUser, ICommandGenerator, IUserTurnInformer, IDisposable {
        public Process EndTurn();
        public void StartTurn();
    }
}
