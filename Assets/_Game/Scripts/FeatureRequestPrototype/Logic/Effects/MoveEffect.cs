using _Game.Scripts.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class MoveEffect : Effect {
        public MoveEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new MoveEffect(data);

        public override EEffectType Type => EEffectType.Move;

        protected override string SpriteName => "move";

        protected override void PerformApplyTo(Rng rng, Employee employee) {
            employee.Position += GetPower(rng);
        }
    }
}