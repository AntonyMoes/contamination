using _Game.Scripts.Data;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class MoveEffect : Effect {
        public MoveEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new MoveEffect(data);

        public override EEffectType Type => EEffectType.Move;

        protected override string SpriteName => "move";

        public override void ApplyTo(Employee employee) {
            throw new System.NotImplementedException();
        }
    }
}