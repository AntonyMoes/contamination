using System;
using _Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Tile : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Preset[] _presets;

        [SerializeField] private Color _highlightedColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _forbiddenColor;
        [SerializeField] private Color _attackColor;

        private Preset _preset;
        public Vector3 Center => _preset.Center.position;
        private Image Border => _preset.Border;

        private readonly Action _mouseEnter;
        public readonly Event MouseEnter;
        private readonly Action _mouseExit;
        public readonly Event MouseExit;

        private Vector2Int _position;
        private int _height;

        public string Tooltip => $"Flats ({_position.x}, {_position.y}, {_height})";

        public Tile() {
            MouseEnter = new Event(out _mouseEnter);
            MouseExit = new Event(out _mouseExit);
        }

        public void Initialize(Vector2Int position, int height) {
            _position = position;
            _height = height;

            if (_preset is { } lastPreset) {
                Unsubscribe(lastPreset);
            }

            foreach (var preset in _presets) {
                var correct = preset.Height == height;
                preset.Object.SetActive(correct);
                if (correct) {
                    _preset = preset;
                    Subscribe(preset);
                }
            }
        }

        private void Subscribe(Preset preset) {
            preset.MouseDetector.MouseEnter.Subscribe(_mouseEnter);
            preset.MouseDetector.MouseExit.Subscribe(_mouseExit);
        }

        private void Unsubscribe(Preset preset) {
            preset.MouseDetector.MouseEnter.Unsubscribe(_mouseEnter);
            preset.MouseDetector.MouseExit.Unsubscribe(_mouseExit);
        }

        public void SetState(State state) {
            Border.gameObject.SetActive(state != State.None);
            Border.color = state switch {
                State.None => Color.clear,
                State.Highlighted => _highlightedColor,
                State.Selected => _selectedColor,
                State.Forbidden => _forbiddenColor,
                State.Attack => _attackColor,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
        
        public enum State {
            None,
            Highlighted,
            Selected,
            Forbidden,
            Attack
        }

        [Serializable]
        private class Preset {
            public int Height;
            public GameObject Object;
            public MouseDetector MouseDetector;
            public Transform Center;
            public Image Border;
        }
    }
}