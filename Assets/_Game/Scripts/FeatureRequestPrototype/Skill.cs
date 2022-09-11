using System;
using System.Linq;
using _Game.Scripts.Data;

namespace _Game.Scripts.FeatureRequestPrototype {
    public class Skill {
        // public SkillData Data => _skillData;
        public string Name => _skillData.Name;
        public string Description => _skillData.Description;

        private readonly SkillData _skillData;

        public Skill(SkillData skillData) {
            _skillData = skillData;
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

        public (int[][] enemyGroups, int[][] allyGroups) GetTargets(Employee user) {
            var enemyGroups = GetGroups(_skillData.EnemyTarget, user, _skillData.AllyTargetExcludeSelf);
            var allyGroups = GetGroups(_skillData.AllyTarget, user, _skillData.AllyTargetExcludeSelf);

            if (enemyGroups.Length != 0 || allyGroups.Length != 0) {
                return (enemyGroups, allyGroups);
            }

            var selfGroups = new[] { new[] { user.Position } };
            return (Array.Empty<int[]>(), selfGroups);

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