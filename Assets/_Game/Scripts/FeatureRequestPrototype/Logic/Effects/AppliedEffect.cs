using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class AppliedEffect {
        public Effect Effect;
        public int RoundsRemaining;

        public AppliedEffect(Effect effect, int duration) {
            Effect = effect;
            RoundsRemaining = duration;
        }

        public string GetSerialization() {
            return Effect.GetSerialization(RoundsRemaining);
        }

        public bool Apply(Rng rng, Employee employee) {
            Effect.ApplyTo(rng, employee, true);
            RoundsRemaining--;

            return RoundsRemaining == 0;
        }
    }
}