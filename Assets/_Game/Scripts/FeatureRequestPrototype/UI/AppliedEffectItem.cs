using System.Collections.Generic;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using GeneralUtils;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class AppliedEffectItem : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private Transform _tooltip;
        [SerializeField] private TooltipItem _tooltipItemPrefab;

        private readonly Dictionary<AppliedEffect, TooltipItem> _effects = new Dictionary<AppliedEffect, TooltipItem>();
        private readonly UpdatedValue<bool> _onHover = new UpdatedValue<bool>(false);

        private void Awake() {
            HoverComponent.Create(_icon, _onHover);
            _onHover.Subscribe(OnItemHover, true);
        }

        private void OnItemHover(bool hover) {
            _tooltip.gameObject.SetActive(hover);
        }

        public void Load(Sprite sprite) {
            _icon.sprite = sprite;
        }

        public void AddEffect(AppliedEffect effect) {
            if (!_effects.ContainsKey(effect)) {
                var tooltipItem = Instantiate(_tooltipItemPrefab, _tooltip);
                tooltipItem.Load(effect);
                _effects.Add(effect, tooltipItem);
            }
        }

        public bool RemoveEffect(AppliedEffect effect) {
            if (_effects.TryGetValue(effect, out var tooltipItem)) {
                Destroy(tooltipItem.gameObject);
                _effects.Remove(effect);
            }

            return _effects.Count == 0;
        }

        private void OnDestroy() {
            foreach (var tooltipItem in _effects.Values) {
                Destroy(tooltipItem.gameObject);
            }

            _effects.Clear();
            _onHover.Clear();
        }
    }
}