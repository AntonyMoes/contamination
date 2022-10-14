using System.Collections.Generic;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class SkillEffectsPanel : UIElement {
        [SerializeField] private EffectItem _effectPrefab;
        [SerializeField] private GameObject _enemyGroup;
        [SerializeField] private Transform _enemyParent;
        [SerializeField] private GameObject _allyGroup;
        [SerializeField] private Transform _allyParent;
        [SerializeField] private GameObject _selfGroup;
        [SerializeField] private Transform _selfParent;

        private readonly List<EffectItem> _effects = new List<EffectItem>();
        private Skill _skill;

        public void Load(Skill skill) {
            if (_skill == skill) {
                return;
            }

            if (_skill != null) {
                Clear();
            }

            _skill = skill;
            FillGroup(_enemyGroup, _enemyParent, skill.EnemyEffects);
            FillGroup(_allyGroup, _allyParent, skill.AllyEffects);
            FillGroup(_selfGroup, _selfParent, skill.SelfEffects);
        }

        private void FillGroup(GameObject group, Transform parent, Effect[] effects) {
            if (effects.Length == 0) {
                group.SetActive(false);
                return;
            }

            foreach (var effect in effects) {
                var item = Instantiate(_effectPrefab, parent);
                item.Load(effect);
                _effects.Add(item);
            }

            group.gameObject.SetActive(true);
        }

        public override void Clear() {
            base.Clear();

            foreach (var effect in _effects) {
                Destroy(effect.gameObject);
            }

            _effects.Clear();
            _skill = null;
        }
    }
}