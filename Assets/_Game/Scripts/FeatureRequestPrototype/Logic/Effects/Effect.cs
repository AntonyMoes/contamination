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

        public string Serialization {
            get {
                var serializedPower = _data.PowerMin == _data.PowerMax
                    ? $"{_data.PowerMin}"
                    : $"{_data.PowerMin}-{_data.PowerMax}";

                var serializedDuration = _data.Duration == 0
                    ? ""
                    : $" for {_data.Duration} rnd";

                return $"{serializedPower} pts{serializedDuration}"; 
            }
        }

        public Sprite Sprite => ArtStorage.Instance.GetSprite(SpriteName);

        protected abstract string SpriteName { get; }

        protected int GetPower(Rng rng) {
            return rng.NextInt(_data.PowerMin, _data.PowerMax);
        }

        public abstract void ApplyTo(Employee employee);
    }
}