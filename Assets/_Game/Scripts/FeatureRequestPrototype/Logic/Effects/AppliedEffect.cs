using System;
using GeneralUtils;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public class AppliedEffect {
        public Effect Effect;
        public int RoundsRemaining;

        private readonly Action _onUpdate;
        public readonly Event OnUpdate;

        public AppliedEffect(Effect effect, int duration) {
            Effect = effect;
            RoundsRemaining = duration;
            OnUpdate = new Event(out _onUpdate);
        }

        public string GetSerialization() {
            return Effect.GetSerialization(RoundsRemaining);
        }

        public bool Apply(Rng rng, Employee employee) {
            Effect.ApplyTo(rng, employee, true);
            RoundsRemaining--;

            _onUpdate();

            return RoundsRemaining == 0;
        }
    }
}