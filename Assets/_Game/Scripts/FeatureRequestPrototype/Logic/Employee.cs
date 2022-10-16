using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Logic {
    public class Employee {
        public string Name => _employeeData.Name;
        public EDepartment Department => _employeeData.Department;

        private int _position;
        public int Position {
            get => _position;
            set => _positionSetter.SetPosition(this, value);
        }

        public readonly Skill[] Skills;

        public int MaxSanity => _employeeData.Sanity;
        public readonly UpdatedValue<int> Sanity;

        private readonly List<AppliedEffect> _appliedEffects = new List<AppliedEffect>();

        private readonly Action<AppliedEffect, bool> _onEffectApplied;
        public readonly Event<AppliedEffect, bool> OnEffectApplied;

        private readonly EmployeeData _employeeData;
        private readonly PositionSetter _positionSetter;

        public Employee(EmployeeData employeeData, PositionSetter positionSetter, int initialPosition) {
            _employeeData = employeeData;
            _positionSetter = positionSetter;
            _position = initialPosition;

            Skills = employeeData.Skills
                .Select(skillData => new Skill(skillData))
                .ToArray();

            Sanity = new UpdatedValue<int>(MaxSanity, SanitySetter);

            OnEffectApplied = new Event<AppliedEffect, bool>(out _onEffectApplied);
        }

        public void AddEffect(AppliedEffect effect) {
            _appliedEffects.Add(effect);
            _onEffectApplied(effect, true);
        }

        public void StartRound(Rng rng) {
            var currentEffects = _appliedEffects.ToArray();
            foreach (var effect in currentEffects) {
                var shouldRemove = effect.Apply(rng, this);
                if (shouldRemove) {
                    _appliedEffects.Remove(effect);
                    _onEffectApplied(effect, false);
                }
            }
        }

        private int SanitySetter(int newSanity) {
            return Mathf.Clamp(newSanity, 0, MaxSanity);
        }
    }
}