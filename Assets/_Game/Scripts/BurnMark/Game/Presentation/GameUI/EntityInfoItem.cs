using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class EntityInfoItem : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _text;

        public string Tooltip => $"{_name}: {_text.text}";

        private string _name;
        // private IValue _value;

        public void Initialize(Sprite icon, string name) {
            _icon.sprite = icon;
            _name = name;
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

        public void SetMultiplicative(float value1, int value2) {
            Set($"{value1:F1}x{value2}");
        }

        private void Set(string text) {
            _text.text = text;
        }

        // private void Set(IValue value) {
        //     _value = value;
        // }
        //
        // private interface IValue {
        //     public string Text { get; }
        //     public string Tooltip { get; }
        // }
        //
        // private class IntValue : IValue {
        //     private readonly int _value;
        //
        //     public string Text => _value.ToString();
        //     public string Tooltip => _value.ToString();
        //
        //     public IntValue(int value) {
        //         _value = value;
        //     }
        // }
        //
        // private class FloatValue : IValue {
        //     private readonly float _value;
        //
        //     public string Text => _value.ToString();
        //     public string Tooltip => _value.ToString();
        //
        //     public FloatValue(float value) {
        //         _value = value;
        //     }
        // }
    }
}