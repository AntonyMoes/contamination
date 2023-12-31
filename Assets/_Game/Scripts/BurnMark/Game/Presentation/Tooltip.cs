using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class Tooltip : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RectTransform _textContainer;
        [SerializeField] private Vector2 _offset;

        private RectTransform _parent;
        private Camera _uiCamera;
        private ITooltipProvider _provider;
        private bool _enabled;

        public void Initialize(Camera uiCamera) {
            _uiCamera = uiCamera;
            _parent = (RectTransform) transform.parent;
        }

        public void Show(ITooltipProvider tooltipProvider, Vector2 screenPosition) {
            if (tooltipProvider != _provider || !_enabled) {
                _enabled = true;
                gameObject.SetActive(true);
                _provider = tooltipProvider;

                _text.text = tooltipProvider.Tooltip;
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform) transform);
            }

            UpdatePosition(screenPosition);
        }

        private void UpdatePosition(Vector2 screenPosition) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_parent, screenPosition,
                _uiCamera, out var localPosition);
            transform.localPosition = localPosition;
            _textContainer.anchoredPosition = GetOffset(localPosition);
        }

        private Vector2 GetOffset(Vector2 localPosition) {
            // var rect = _textContainer.rect.m 
            var parentRect = _parent.rect;
            var rect = _textContainer.rect;
            var offsetPosition = localPosition + _offset;
            var maxOffset = parentRect.max - (offsetPosition + rect.max);
            var minOffset = (offsetPosition + rect.min) - parentRect.min;
            var offset = _offset;
            if (maxOffset.x < 0) {
                offset.x += maxOffset.x;
            }
            if (maxOffset.y < 0) {
                offset.y += maxOffset.y;
            }
            if (minOffset.x < 0) {
                offset.x -= minOffset.x;
            }
            if (minOffset.y < 0) {
                offset.y -= minOffset.y;
            }
            return offset;
        }

        public void Hide() {
            _enabled = false;
            gameObject.SetActive(false);
        }
    }
}