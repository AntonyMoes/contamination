using System.Collections.Generic;
using _Game.Scripts.Scheduling;
using UnityEngine.EventSystems;

namespace _Game.Scripts.BurnMark.Game {
    public class PointerRaycastProvider : IFrameProcessor {
        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
        public IReadOnlyList<RaycastResult> RaycastResults => _raycastResults;

        public void ProcessFrame(float deltaTime) {
            var eventData = new PointerEventData(EventSystem.current) {
                position = UnityEngine.Input.mousePosition
            };

            _raycastResults.Clear();
            EventSystem.current.RaycastAll(eventData, _raycastResults);
        }
    }
}