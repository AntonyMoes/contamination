using System;
using _Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Tile : MonoBehaviour {
        [SerializeField] private Preset[] _presets;

        [SerializeField] private Color _highlightedColor;
        [SerializeField] private Color _selectedColor;

        private Preset _preset;
        public Vector3 Center => _preset.Center.position;
        private Image Border => _preset.Border;

        private readonly Action _mouseEnter;
        public readonly Event MouseEnter;
        private readonly Action _mouseExit;
        public readonly Event MouseExit;

        public Tile() {
            MouseEnter = new Event(out _mouseEnter);
            MouseExit = new Event(out _mouseExit);
        }

        public void Initialize(int height) {
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
            switch (state) {
                case State.None:
                    Border.gameObject.SetActive(false);
                    break;
                case State.Highlighted:
                    Border.gameObject.SetActive(true);
                    Border.color = _highlightedColor;
                    break;
                case State.Selected:
                    Border.gameObject.SetActive(true);
                    Border.color = _selectedColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
        
        public enum State {
            None,
            Highlighted,
            Selected
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