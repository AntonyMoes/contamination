using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Mechanics {
    public static class Attacking {
        public static bool CanAttack(FieldAccessor accessor, IReadOnlyEntity entity, IReadOnlyEntity target) {
            var from = entity.GetReadOnlyComponent<PositionData>()!.Data.Position;
            var to = target.GetReadOnlyComponent<PositionData>()!.Data.Position;

            var attackData = entity.GetReadOnlyComponent<AttackData>()!.Data;
            return Position.Distance(from, to) <= attackData.Range;
        }

        public static float CalculateDamage(AttackData attackData, HealthData healthData) {
            var damageReduction = Mathf.Max(healthData.Armor - attackData.ArmorPiercing, 0);
            var actualDamage = Mathf.Max(attackData.Damage - damageReduction, 0);
            return actualDamage * attackData.Attacks;
        }
    }
}