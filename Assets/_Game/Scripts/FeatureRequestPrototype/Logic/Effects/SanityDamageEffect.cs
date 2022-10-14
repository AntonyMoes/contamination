using _Game.Scripts.Data;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class SanityDamageEffect : Effect {
        public SanityDamageEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new SanityDamageEffect(data);

        public override EEffectType Type => EEffectType.SanityDamage;

        protected override string SpriteName => "damage";

        public override void ApplyTo(Employee employee) {
            throw new System.NotImplementedException();
        }
    }
}