using _Game.Scripts.FeatureRequestPrototype.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class BurnoutEffect : Effect {
        public BurnoutEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new BurnoutEffect(data);

        public override EEffectType Type => EEffectType.Burnout;
        public override EEffectGroup Group => EEffectGroup.Burnout;

        protected override void PerformApplyTo(Rng rng, Employee employee) {
            employee.Sanity.Value -= GetPower(rng);
        }
    }
}