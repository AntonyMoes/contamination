using _Game.Scripts.Data;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class MoveEffect : Effect {
        public MoveEffect(EffectData data) : base(data) { }
        public override Effect CreateFrom(EffectData data) => new MoveEffect(data);

        public override EEffectType Type => EEffectType.Move;
        public override EEffectGroup Group => EEffectGroup.Move;

        protected override void PerformApplyTo(Rng rng, Employee employee) {
            employee.Position += GetPower(rng);
        }
    }
}