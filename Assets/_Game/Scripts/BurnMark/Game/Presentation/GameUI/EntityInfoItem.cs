using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class EntityInfoItem : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        public void Initialize(Sprite icon) {
            _icon.sprite = icon;
        }

        public void Set(int value) {
            Set(value.ToString());
        }

        public void Set(float value) {
            Set(value.ToString("F1"));
        }

        public void SetDivisive(float value1, float value2) {
            Set($"{value1:F1}/{value2:F1}");
        }

        public void SetMultiplicative( float value1, int value2) {
            Set($"{value1:F1}x{value2}");
        }

        private void Set(string text) {
            _text.text = text;
        }
    }
}