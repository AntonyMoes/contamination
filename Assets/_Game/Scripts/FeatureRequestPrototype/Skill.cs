using System;
using System.Linq;
using _Game.Scripts.Data;

namespace _Game.Scripts.FeatureRequestPrototype {
    public class Skill {
        private readonly SkillData _skillData;

        public int[] Positions => _skillData.SelfPositions;

        public Skill(SkillData skillData) {
            _skillData = skillData;
        }

        public bool CanBeUsed(Employee user, Employee[] enemies, Employee[] allies) {
            if (!_skillData.SelfPositions.Contains(user.Position))
                return false;

            if (!CheckTarget(_skillData.EnemyTarget, enemies)) {
                return false;
            }

            if (!CheckTarget(_skillData.AllyTarget, allies)) {
                return false;
            }

            return true;

            static bool CheckTarget(Target targetInfo, Employee[] employees) {
                if (!(targetInfo is { } target)) {
                    return true;
                }

                var validEmployeesPresent = target.Exclusive
                    ? employees.Any(e => target.Positions.Contains(e.Position))
                    : target.Positions.All(p => employees.Any(e => e.Position == p));
                return validEmployeesPresent;
            }
        }

        public (int[][] enemyGroups, int[][] allyGroups) GetTargets(Employee user) {
            var enemyGroups = GetGroups(_skillData.EnemyTarget);
            var allyGroups = GetGroups(_skillData.AllyTarget);

            if (enemyGroups.Length != 0 || allyGroups.Length != 0) {
                return (enemyGroups, allyGroups);
            }

            var selfGroups = new[] { new[] { user.Position } };
            return (Array.Empty<int[]>(), selfGroups);

            static int[][] GetGroups(Target target) {
                if (target == null) {
                    return Array.Empty<int[]>();
                }

                return target.Exclusive
                    ? target.Positions.Select(p => new[] { p }).ToArray()
                    : new[] { target.Positions };
            }
        }
    }
}