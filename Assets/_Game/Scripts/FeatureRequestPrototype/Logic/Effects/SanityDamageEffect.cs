using _Game.Scripts.FeatureRequestPrototype.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class SanityDamageEffect : Effect {
        public SanityDamageEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new SanityDamageEffect(data);

        public override EEffectType Type => EEffectType.SanityDamage;
        public override EEffectGroup Group => EEffectGroup.SanityDamage;

        protected override void PerformApplyTo(Rng rng, Employee employee) {
            employee.Sanity.Value -= GetPower(rng);
        }
    }
}