using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.Data;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using _Game.Scripts.FeatureRequestPrototype.Logic.Skills;
using GeneralUtils;
using JetBrains.Annotations;
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

        public readonly ISkill[] Skills;

        public int MaxSanity => _employeeData.Sanity;
        public readonly UpdatedValue<int> Sanity;

        public int MoveForward => _employeeData.MoveForward;
        public int MoveBackward => _employeeData.MoveBackward;

        private readonly List<AppliedEffect> _appliedEffects = new List<AppliedEffect>();

        private readonly Action<AppliedEffect, bool> _onEffectApplied;
        public readonly Event<AppliedEffect, bool> OnEffectApplied;

        private readonly EmployeeData _employeeData;
        private readonly PositionSetter _positionSetter;

        public Employee(EmployeeData employeeData, PositionSetter positionSetter, out Action<int> positionUpdater) {
            _employeeData = employeeData;
            _positionSetter = positionSetter;

            Skills = employeeData.Skills
                .Select(skillData => new Skill(skillData))
                .Append<ISkill>(new MoveSelfSkill(this))
                .ToArray();

            Sanity = new UpdatedValue<int>(MaxSanity, SanitySetter);

            OnEffectApplied = new Event<AppliedEffect, bool>(out _onEffectApplied);

            positionUpdater = position => _position = position;
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

    public static class EmployeeHelper {
        [CanBeNull]
        public static Employee WithPosition(this IEnumerable<Employee> employees, int position) {
            return employees.FirstOrDefault(employee => employee.Position == position);
        }
    }
}