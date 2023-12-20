using _Game.Scripts.Scheduling;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class FieldCamera : MonoBehaviour, IFrameProcessor {
        [SerializeField] private Camera _camera;
        [SerializeField] private Vector3 _minZoomLocalPosition;
        [SerializeField] private Vector3 _maxZoomLocalPosition;
        [SerializeField] private float _moveSpeed;

        public Camera Camera => _camera;

        private const float ZoomDelta = 0.1f;
        private float _zoom;
        private Vector3 ZoomPosition => Vector3.Lerp(_minZoomLocalPosition, _maxZoomLocalPosition, _zoom);
        private float ZoomSpeedScale => ZoomPosition.magnitude / _minZoomLocalPosition.magnitude;

        private IUpdatedValue<Vector2Int> _edgeVector;
        private IUpdatedValue<Vector2> _mouseDelta;
        private Vector3 _minPosition;
        private Vector3 _maxPosition;
        private Vector3 _targetPosition;
        private bool _drag;

        public void Initialize(IUpdatedValue<Vector2Int> edgeVector, IUpdatedValue<Vector2> mouseDelta) {
            _edgeVector = edgeVector;
            _mouseDelta = mouseDelta;
        }

        public void SetBounds(Vector3 minPosition, Vector3 maxPosition) {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
        }

        public void SetPosition(Vector3 position) {
            SetTargetPosition(position);
            transform.position = _targetPosition;
        }

        public void SetZoom(float zoom) {
            _zoom = Mathf.Clamp01(zoom);
            _camera.transform.localPosition = ZoomPosition;
        }

        public void OnZoom(bool zoomIn) {
            SetZoom(_zoom + ZoomDelta * (zoomIn ? 1 : -1));
        }

        public void ToggleDrag(bool drag) {
            _drag = drag;
        }

        public void ProcessFrame(float deltaTime) {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, 0.2f);

            if (_drag) {
                Debug.Log(_mouseDelta.Value);

                var currentPosition = transform.position;
                var dragPosition = _camera.ScreenToWorldPoint(_camera.WorldToScreenPoint(currentPosition) + (Vector3) _mouseDelta.Value);
                SetTargetPosition(_targetPosition - (dragPosition - currentPosition));
            }
            
            if (_edgeVector.Value == Vector2Int.zero) {
                return;
            }

            var positionDelta = (Vector2) _edgeVector.Value * (_moveSpeed * ZoomSpeedScale);
            SetTargetPosition(_targetPosition + new Vector3(positionDelta.x, 0f, positionDelta.y));
        }

        private void SetTargetPosition(Vector3 targetPosition) {
            _targetPosition = new Vector3(
                Mathf.Clamp(targetPosition.x, _minPosition.x, _maxPosition.x),
                Mathf.Clamp(targetPosition.y, _minPosition.y, _maxPosition.y),
                Mathf.Clamp(targetPosition.z, _minPosition.z, _maxPosition.z)
            );
        }
    }
}