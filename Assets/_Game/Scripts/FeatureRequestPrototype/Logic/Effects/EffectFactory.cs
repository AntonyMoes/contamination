using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.Data;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public static class EffectFactory {
        public static Effect Create(EffectData data) {
            return Prototypes.First(prototype => prototype.Type == data.EffectType).CreateFrom(data);
        }

        private static readonly HashSet<Effect> Prototypes = new HashSet<Effect>() {
            new SanityDamageEffect(null),
            new SanityHealingEffect(null),
            new BurnoutEffect(null),
            new MoveEffect(null)
        };
    }
}