using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class EntityIcon : MonoBehaviour {
        [SerializeField] private Image _image;
        [SerializeField] private ProgressBar _progressBar;

        private Transform _target;
        private Camera _fieldCamera;
        private Camera _uiCamera;
        private IReadOnlyComponent<HealthData> _healthComponent;

        public void Initialize(Transform target, Sprite icon, Camera fieldCamera, Camera uiCamera,
            IReadOnlyComponent<HealthData> healthComponent) {
            _target = target;
            _image.sprite = icon;
            _fieldCamera = fieldCamera;
            _uiCamera = uiCamera;
            _healthComponent = healthComponent;
            _progressBar.Load(0, healthComponent.Data.MaxHealth);
            _healthComponent.OnDataUpdate.Subscribe(OnHealthUpdate);
            OnHealthUpdate(null, _healthComponent);
        }

        private void OnHealthUpdate(HealthData? _, IReadOnlyComponent<HealthData> __) {
            _progressBar.CurrentValue = _healthComponent.Data.Health;
        }

        public void Clear() {
            _healthComponent.OnDataUpdate.Unsubscribe(OnHealthUpdate);
        }

        private void Update() {
            var viewportPoint = _fieldCamera.WorldToViewportPoint(_target.position);
            var screenPoint = _uiCamera.ViewportToScreenPoint(viewportPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform.parent, screenPoint,
                _uiCamera, out var localPoint);
            ((RectTransform) transform).anchoredPosition = localPoint;
        }
    }
}