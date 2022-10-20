using System.Collections.Generic;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using _Game.Scripts.FeatureRequestPrototype.Logic.Skills;
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
        private ISkill _skill;

        public void Load(ISkill skill) {
            if (_skill == skill) {
                return;
            }

            if (_skill != null) {
                Clear();
            }

            _skill = skill;
            FillGroup(_enemyGroup, _enemyParent, skill.Effects, ESkillTarget.Enemy);
            FillGroup(_allyGroup, _allyParent, skill.Effects, ESkillTarget.Ally);
            FillGroup(_selfGroup, _selfParent, skill.Effects, ESkillTarget.Self, ESkillTarget.MovePosition);
        }

        private void FillGroup(GameObject group, Transform parent, IDictionary<ESkillTarget, Effect[]> effects, params ESkillTarget[] targets) {
            group.SetActive(false);

            foreach (var target in targets) {
                if (!effects.TryGetValue(target, out var targetEffects) || targetEffects.Length == 0) {
                    continue;
                }

                group.gameObject.SetActive(true);

                foreach (var effect in targetEffects) {
                    var item = Instantiate(_effectPrefab, parent);
                    item.Load(effect);
                    _effects.Add(item);
                }
            }
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