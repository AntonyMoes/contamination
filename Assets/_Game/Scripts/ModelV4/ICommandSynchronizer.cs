using System;

namespace _Game.Scripts.ModelV4 {
    public interface ICommandSynchronizer {
        void WaitForAllCommandsFinished(Action onDone);
    }
}
