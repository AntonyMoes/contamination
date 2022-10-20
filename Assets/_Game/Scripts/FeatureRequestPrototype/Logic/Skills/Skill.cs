using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.Data;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Skills {
    public class Skill : ISkill {
        public string Name => _skillData.Name;
        public string Description => _skillData.Description;

        private readonly Dictionary<ESkillTarget, Effect[]> _effects = new Dictionary<ESkillTarget, Effect[]>();
        public IDictionary<ESkillTarget, Effect[]> Effects => _effects;

        private readonly SkillData _skillData;

        public Skill(SkillData skillData) {
            _skillData = skillData;
            _effects.Add(ESkillTarget.Enemy, CreateEffects(skillData.EnemyTarget?.Effects));
            _effects.Add(ESkillTarget.Ally, CreateEffects(skillData.AllyTarget?.Effects));
            _effects.Add(ESkillTarget.Self, CreateEffects(skillData.SelfEffects));

            static Effect[] CreateEffects(EffectData[] datas)
                => (datas ?? new EffectData[]{}).Select(EffectFactory.Create).ToArray();
        }

        public void Apply(Rng rng, Employee employee, Employee[] enemies, Employee[] allies) {
            var rngResult = rng.NextInt(1, Constants.MaxAccuracy);
            if (rngResult > _skillData.Accuracy) {
                Debug.LogWarning("Miss!");
                return;
            }

            var targetMapping = new Dictionary<ESkillTarget, Employee[]> {
                [ESkillTarget.Enemy] = enemies,
                [ESkillTarget.Ally] = allies,
                [ESkillTarget.Self] = new[] { employee }
            };

            foreach (var (target, effects) in Effects) {
                targetMapping[target].ForEach(em => effects.ForEach(eff => eff.ApplyTo(rng, em)));
            }
        }

        public bool CanBeUsed(Employee user, Employee[] enemies, Employee[] allies) {
            if (!_skillData.SelfPositions.Contains(user.Position))
                return false;

            if (!CheckTarget(_skillData.EnemyTarget, enemies, user)) {
                return false;
            }

            if (!CheckTarget(_skillData.AllyTarget, allies, user, _skillData.AllyTargetExcludeSelf)) {
                return false;
            }

            return true;

            static bool CheckTarget(Target targetInfo, Employee[] employees, Employee user, bool allyTargetAndExcludeSelf = false) {
                if (!(targetInfo is { } target)) {
                    return true;
                }

                return employees.Any(e => 
                    !(allyTargetAndExcludeSelf && e == user) && target.Positions.Contains(e.Position));
            }
        }

        public IDictionary<ESkillTarget, int[][]> GetTargets(Employee user) {
            var enemyGroups = GetGroups(_skillData.EnemyTarget, user);
            var allyGroups = GetGroups(_skillData.AllyTarget, user, _skillData.AllyTargetExcludeSelf);
            var selfGroups = new[] { new[] { user.Position } };

            return new Dictionary<ESkillTarget, int[][]> {
                [ESkillTarget.Enemy] = enemyGroups,
                [ESkillTarget.Ally] = enemyGroups.Length != 0 || allyGroups.Length != 0 ? allyGroups : selfGroups
            };

            static int[][] GetGroups(Target target, Employee user, bool allyTargetAndExcludeSelf = false) {
                if (target == null) {
                    return Array.Empty<int[]>();
                }

                return target.Exclusive
                    ? target.Positions
                        .Where(p => !(allyTargetAndExcludeSelf && p == user.Position))
                        .Select(p => new[] { p })
                        .ToArray()
                    : new[] { target.Positions };
            }
        }
    }
}