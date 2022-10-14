using _Game.Scripts.Data;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class SanityHealingEffect : Effect {
        public SanityHealingEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new SanityHealingEffect(data);

        public override EEffectType Type => EEffectType.SanityHealing;

        protected override string SpriteName => "heal";

        public override void ApplyTo(Employee employee) {
            throw new System.NotImplementedException();
        }
    }
}