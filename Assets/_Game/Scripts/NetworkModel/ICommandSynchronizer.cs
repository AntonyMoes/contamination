using System;

namespace _Game.Scripts.NetworkModel {
    public interface ICommandSynchronizer {
        void WaitForAllCommandsFinished(Action onDone);
    }
}
