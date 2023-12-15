using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class EntityInfoItem : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        public void SetIcon(Sprite icon) {
            _icon.sprite = icon;
        }

        public void Initialize(int value) {
            _text.text = value.ToString();
        }

        public void Initialize(float value) {
            _text.text = value.ToString("F1");
        }

        public void InitializeDivisive(float value1, float value2) {
            _text.text = $"{value1:F1}/{value2:F1}";
        }

        public void InitializeMultiplicative( float value1, int value2) {
            _text.text = $"{value1:F1}x{value2}";
        }
    }
}