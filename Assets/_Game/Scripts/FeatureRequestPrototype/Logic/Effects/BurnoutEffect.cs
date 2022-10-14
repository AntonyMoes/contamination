using _Game.Scripts.Data;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class BurnoutEffect : Effect {
        public BurnoutEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new BurnoutEffect(data);

        public override EEffectType Type => EEffectType.Burnout;

        protected override string SpriteName => "burnout";

        public override void ApplyTo(Employee employee) {
            throw new System.NotImplementedException();
        }
    }
}