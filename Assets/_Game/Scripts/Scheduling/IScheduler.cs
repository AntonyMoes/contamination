using System;
using System.Collections;

namespace _Game.Scripts.Scheduling {
    public interface IScheduler {
        void RegisterFrameProcessor(IFrameProcessor frameProcessor);
        void UnregisterFrameProcessor(IFrameProcessor frameProcessor);
        void RegisterPhysicsFrameProcessor(IPhysicsFrameProcessor frameProcessor);
        void UnregisterPhysicsFrameProcessor(IPhysicsFrameProcessor frameProcessor);
        public Action RunCoroutine(IEnumerator enumerator);
    }
}