using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts.BurnMark.Game {
    public class Input : IFrameProcessor {
        private readonly UpdatedValue<bool> _selectionButton = new UpdatedValue<bool>();
        public IUpdatedValue<bool> SelectionButton => _selectionButton;
        private readonly UpdatedValue<bool> _actionButton = new UpdatedValue<bool>();
        public  IUpdatedValue<bool> ActionButton => _actionButton;

        private const int SelectionMouseButton = 0;
        private const int ActionMouseButton = 1;

        private static readonly int UILayer = LayerMask.NameToLayer("UI");

        public void ProcessFrame(float deltaTime) {
            if (IsPointerOverUIElement(GetEventSystemRaycastResults())) {
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(SelectionMouseButton)) {
                _selectionButton.Value = true;
            }

            if (UnityEngine.Input.GetMouseButtonUp(SelectionMouseButton)) {
                _selectionButton.Value = false;
            }

            if (UnityEngine.Input.GetMouseButtonDown(ActionMouseButton)) {
                _actionButton.Value = true;
            }

            if (UnityEngine.Input.GetMouseButtonUp(ActionMouseButton)) {
                _actionButton.Value = false;
            }
        }
 
        private static bool IsPointerOverUIElement(IEnumerable<RaycastResult> raycastResults) {
            return raycastResults.Any(result => result.gameObject.layer == UILayer);
        }
 
        private static List<RaycastResult> GetEventSystemRaycastResults() {
            var eventData = new PointerEventData(EventSystem.current) {
                position = UnityEngine.Input.mousePosition
            };
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            return raycastResults;
        }
    }
}