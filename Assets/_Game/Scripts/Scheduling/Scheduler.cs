using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Scheduling {
    public class Scheduler : MonoBehaviour, IScheduler {
        private readonly List<IFrameProcessor> _frameProcessors = new List<IFrameProcessor>();
        private readonly List<IPhysicsFrameProcessor> _physicsFrameProcessors = new List<IPhysicsFrameProcessor>();

        public void RegisterFrameProcessor(IFrameProcessor frameProcessor) {
            if (!_frameProcessors.Contains(frameProcessor)) {
                _frameProcessors.Add(frameProcessor);
            }
        }

        public void UnregisterFrameProcessor(IFrameProcessor frameProcessor) {
            _frameProcessors.Remove(frameProcessor);
        }

        public void RegisterPhysicsFrameProcessor(IPhysicsFrameProcessor frameProcessor) {
            if (!_physicsFrameProcessors.Contains(frameProcessor)) {
                _physicsFrameProcessors.Add(frameProcessor);
            }
        }

        public void UnregisterPhysicsFrameProcessor(IPhysicsFrameProcessor frameProcessor) {
            _physicsFrameProcessors.Remove(frameProcessor);
        }

        public Action RunCoroutine(IEnumerator enumerator) {
            var coroutine = StartCoroutine(enumerator);
            return () => StopCoroutine(coroutine);
        }

        private void Update() {
            var deltaTime = Time.deltaTime;
            foreach (var frameProcessor in _frameProcessors) {
                frameProcessor.ProcessFrame(deltaTime);
            }
        }

        private void FixedUpdate() {
            var deltaTime = Time.fixedDeltaTime;
            foreach (var frameProcessor in _physicsFrameProcessors) {
                frameProcessor.ProcessPhysicsFrame(deltaTime);
            }
        }
    }
}