using _Game.Scripts.FeatureRequestPrototype.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class SanityHealingEffect : Effect {
        public SanityHealingEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new SanityHealingEffect(data);

        public override EEffectType Type => EEffectType.SanityHealing;
        public override EEffectGroup Group => EEffectGroup.SanityHealing;

        protected override void PerformApplyTo(Rng rng, Employee employee) {
            employee.Sanity.Value += GetPower(rng);
        }
    }
}