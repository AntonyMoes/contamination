using System.Collections.Generic;
using _Game.Scripts.FeatureRequestPrototype.Logic.Effects;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Skills {
    public interface ISkill {
        public string Name { get; }
        public string Description { get; }
        public IDictionary<ESkillTarget, Effect[]> Effects { get; }

        public void Apply(Rng rng, Employee employee, Employee[] enemies, Employee[] allies);

        public bool CanBeUsed(Employee user, Employee[] enemies, Employee[] allies);

        public IDictionary<ESkillTarget, int[][]> GetTargets(Employee user);
    }
}