using System;

namespace _Game.Scripts.FeatureRequestPrototype.Logic.Effects {
    public enum EEffectGroup {
        SanityDamage,
        SanityHealing,
        Burnout,
        Move,
        Buff,
        Debuff
    }

    public static class EffectGroupHelper {
        public static string GetSpriteName(this EEffectGroup group) {
            return group switch {
                EEffectGroup.SanityDamage => "damage",
                EEffectGroup.SanityHealing => "heal",
                EEffectGroup.Burnout => "burnout",
                EEffectGroup.Move => "move",
                EEffectGroup.Buff => "buff",
                EEffectGroup.Debuff => "debuff",
                _ => throw new ArgumentOutOfRangeException(nameof(group), group, null)
            };
        }
    }
}