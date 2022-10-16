using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using _Game.Scripts.FeatureRequestPrototype.UI;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.GameObjects {
    public class EmployeeObject : MonoBehaviour {
        [SerializeField] private EmployeeSelector _selector;
        [SerializeField] private ProgressBar _sanityBar;

        [SerializeField] private Transform _effectContainer;
        [SerializeField] private AppliedEffectItem _effectPrefab;

        public EmployeeSelector Selector => _selector;

        public Employee Employee { get; private set; }

        private readonly Dictionary<EEffectGroup, AppliedEffectItem> _effects =
            new Dictionary<EEffectGroup, AppliedEffectItem>();

        public void Load(Employee employee) {
            Employee = employee;

            _sanityBar.Load(0, Employee.MaxSanity);
            Employee.Sanity.Subscribe(value => _sanityBar.CurrentValue = value, true);
            Employee.OnEffectApplied.Subscribe(OnEffectApplied);
        }

        private void Awake() {
            Selector.SetActive(false);
        }

        private void OnEffectApplied(AppliedEffect effect, bool applied) {
            var hasItem = _effects.TryGetValue(effect.Effect.Group, out var item);
            if (applied) {
                if (hasItem) {
                    item.AddEffect(effect);
                } else {
                    var newItem = Instantiate(_effectPrefab, _effectContainer);
                    newItem.Load(effect.Effect.Sprite);
                    newItem.AddEffect(effect);
                    _effects.Add(effect.Effect.Group, newItem);
                }
            } else if (hasItem) {
                var shouldDelete = item.RemoveEffect(effect);
                if (shouldDelete) {
                    Destroy(item.gameObject);
                    _effects.Remove(effect.Effect.Group);
                }
            }
        }
    }

    public static class EmployeeObjectHelper {
        public static EmployeeObject WithPosition(this IEnumerable<EmployeeObject> employees, int position) {
            return employees.First(employee => employee.Employee.Position == position);
        }
    }
}