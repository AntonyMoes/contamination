using _Game.Scripts.Data;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public abstract class Effect {
        private readonly EffectData _data;

        protected Effect(EffectData data) {
            _data = data;
        }

        public abstract Effect CreateFrom(EffectData data);

        public abstract EEffectType Type { get; }
        public Sprite Sprite => ArtStorage.Instance.GetSprite(SpriteName);

        protected abstract string SpriteName { get; }

        public string GetSerialization(int? remainingDuration = null) {
            var serializedPower = _data.PowerMin == _data.PowerMax
                ? $"{_data.PowerMin}"
                : $"{_data.PowerMin}-{_data.PowerMax}";

            var duration = remainingDuration ?? _data.Duration;
            var serializedDuration = duration == 0
                ? ""
                : $" for {_data.Duration} rnd";

            return $"{serializedPower} pts{serializedDuration}"; 
        }

        protected int GetPower(Rng rng) {
            return rng.NextInt(_data.PowerMin, _data.PowerMax);
        }

        public void ApplyTo(Rng rng, Employee employee, bool ignoreDuration = false) {
            if (!ignoreDuration && _data.Duration > 0) {
                employee.AddEffect(new AppliedEffect(this, _data.Duration));
            } else {
                PerformApplyTo(rng, employee);
            }
        }

        protected abstract void PerformApplyTo(Rng rng, Employee employee);
    }
}