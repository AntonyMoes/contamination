using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Configs.Terrain;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.TerrainFeatures;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField {
    public class Tile : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Preset[] _presets;
        [SerializeField] private FeaturePreset[] _featurePresets;
        [SerializeField] private GameObject _canvas;
        [SerializeField] private Image _border;

        [SerializeField] private Color _highlightedColor;
        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _forbiddenColor;
        [SerializeField] private Color _attackColor;

        private Preset _preset;
        public Vector3 Center => _preset.Center.position;
        private Image Border => _border;

        private readonly Action _mouseEnter;
        public readonly Event MouseEnter;
        private readonly Action _mouseExit;
        public readonly Event MouseExit;

        private Vector2Int _position;
        private IReadOnlyComponent<TerrainData> _terrain;

        public string Tooltip =>
            $"({_position.x}, {_position.y}, {_terrain.Data.Height})" + (_terrain.Data.Features.Length != 0
                ? "\n" + string.Join("\n", _terrain.Data.Features.Select(f => f.Tooltip))
                : "");

        public Tile() {
            MouseEnter = new Event(out _mouseEnter);
            MouseExit = new Event(out _mouseExit);
        }

        public void Initialize(Vector2Int position, IReadOnlyComponent<TerrainData> terrain, GameDataReadAPI readAPI) {
            _position = position;
            _terrain = terrain;

            if (_preset is { } lastPreset) {
                Unsubscribe(lastPreset);
            }

            foreach (var preset in _presets) {
                var correct = preset.Height == _terrain.Data.Height;
                preset.Object.SetActive(correct);
                if (correct) {
                    _preset = preset;
                    _canvas.transform.position = Center;
                    Subscribe(preset);
                }
            }

            foreach (var featurePreset in _featurePresets) {
                featurePreset.Preset.Clear();
            }

            foreach (var featurePreset in _featurePresets) {
                if (_terrain.Data.Features.Contains(featurePreset.Feature)) {
                    featurePreset.Preset.Init(Center, terrain, readAPI);
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
        }

        [Serializable]
        private class FeaturePreset {
            public TerrainFeaturePreset Preset;
            public TerrainFeatureConfig Feature;
        }
    }
}