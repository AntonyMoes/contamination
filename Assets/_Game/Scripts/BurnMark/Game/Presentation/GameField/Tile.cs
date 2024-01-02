using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Configs.Terrain;
using _Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Tile : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Preset[] _presets;
        [SerializeField] private FeaturePreset[] _featurePresets;

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
        private TerrainData _data;

        public string Tooltip =>
            $"({_position.x}, {_position.y}, {_data.Height})" + (_data.Features.Length != 0
                ? "\n" + string.Join("\n", _data.Features.Select(f => f.Name))
                : "");

        public Tile() {
            MouseEnter = new Event(out _mouseEnter);
            MouseExit = new Event(out _mouseExit);
        }

        public void Initialize(Vector2Int position, TerrainData data) {
            _position = position;
            _data = data;

            if (_preset is { } lastPreset) {
                Unsubscribe(lastPreset);
            }

            foreach (var preset in _presets) {
                var correct = preset.Height == _data.Height;
                preset.Object.SetActive(correct);
                if (correct) {
                    _preset = preset;
                    Subscribe(preset);
                }
            }

            foreach (var featurePreset in _featurePresets) {
                if (_data.Features.Contains(featurePreset.Feature)) {
                    featurePreset.Object.gameObject.SetActive(true);
                    featurePreset.Object.transform.position = Center;
                } else {
                    featurePreset.Object.gameObject.SetActive(false);
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

        [Serializable]
        private class FeaturePreset {
            public Transform Object;
            public TerrainFeatureConfig Feature;
        }
    }
}