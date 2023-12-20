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
        public IUpdatedValue<bool> ActionButton => _actionButton;

        private readonly UpdatedValue<Vector2Int> _edgeVector = new UpdatedValue<Vector2Int>();
        public IUpdatedValue<Vector2Int> EdgeVector => _edgeVector;

        private readonly UpdatedValue<Vector2> _mouseMovement = new UpdatedValue<Vector2>();
        public IUpdatedValue<Vector2> MouseMovement => _mouseMovement;

        private readonly System.Action<bool> _zoom;
        public readonly Event<bool> Zoom;

        private const int SelectionMouseButton = 0;
        private const int ActionMouseButton = 1;

        private static readonly int UILayer = LayerMask.NameToLayer("UI");

        private Vector3 _lastMousePosition;

        public Input() {
            Zoom = new Event<bool>(out _zoom);
        }

        public void ProcessFrame(float deltaTime) {
            var mousePosition = UnityEngine.Input.mousePosition;
            _mouseMovement.Value =  mousePosition - _lastMousePosition;
            _lastMousePosition = mousePosition;

            UpdateEdgeVector();
            
            if (IsPointerOverUIElement(GetEventSystemRaycastResults())) {
                return;
            }

            if (UnityEngine.Input.mouseScrollDelta == Vector2.up) {
                _zoom(true);
            } else if (UnityEngine.Input.mouseScrollDelta == Vector2.down) {
                _zoom(false);
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

        private void UpdateEdgeVector() {
            var screenSize = new Vector2(Screen.width, Screen.height);
            var mousePosition = (Vector2) UnityEngine.Input.mousePosition;
            var x = Near(mousePosition.x, 0)
                ? -1
                : Near(mousePosition.x, screenSize.x)
                    ? 1
                    : 0;
            var y = Near(mousePosition.y, 0)
                ? -1
                : Near(mousePosition.y, screenSize.y)
                    ? 1
                    : 0;
            _edgeVector.Value = new Vector2Int(x, y);

            static bool Near(float a, float b) => Mathf.Abs(b - a) < 10f;
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