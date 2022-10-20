using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Skills {
    public class MoveSelfSkill : ISkill {
        public string Name => "Move";
        public string Description => "";

        public IDictionary<ESkillTarget, Effect[]> Effects => new Dictionary<ESkillTarget, Effect[]> {
            [ESkillTarget.MovePosition] = new Effect[] { _effect }
        };

        private readonly MoveSelfEffect _effect;

        public MoveSelfSkill(Employee employee) {
            _effect = new MoveSelfEffect(employee);
        }

        public void Apply(Rng rng, Employee employee, Employee[] enemies, Employee[] allies) {
            _effect.SetMoveTarget(allies.First());
            _effect.ApplyTo(rng, employee);
        }

        public bool CanBeUsed(Employee user, Employee[] enemies, Employee[] allies) {
            for (var position = Constants.MinPosition; position <= Constants.MaxPosition; position++) {
                if (allies.WithPosition(position) != null) {
                    return true;
                }
            }

            return false;
        }

        public IDictionary<ESkillTarget, int[][]> GetTargets(Employee user) {
            var positions = new List<int>();
            for (var position = Constants.MinPosition; position <= Constants.MaxPosition; position++) {
                if (position >= user.Position - user.MoveForward
                    && position <= user.Position + user.MoveBackward
                    && position != user.Position) {
                    positions.Add(position);
                }
            }

            return new Dictionary<ESkillTarget, int[][]> {
                [ESkillTarget.MovePosition] = positions.Select(pos => new[] { pos }).ToArray()
            };
        }
    }
}