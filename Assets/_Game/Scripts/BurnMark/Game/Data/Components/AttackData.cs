using System;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    [Serializable]
    public struct AttackData : ISame<AttackData> {
        public int Damage;
        public int Attacks;
        public int ArmorPiercing;
        public int Range;
        [HideInInspector] public bool CanAttack;

        public bool IsSame(AttackData other) {
            return Damage == other.Damage
                   && Attacks == other.Attacks
                   && ArmorPiercing == other.ArmorPiercing
                   && Range == other.Range
                   && CanAttack == other.CanAttack;
        }

        public AttackData ResetAttacking() {
            return new AttackData {
                Damage = Damage,
                Attacks = Attacks,
                ArmorPiercing = ArmorPiercing,
                Range = Range,
                CanAttack = true
            };
        }

        public AttackData Attack() {
            return new AttackData {
                Damage = Damage,
                Attacks = Attacks,
                ArmorPiercing = ArmorPiercing,
                Range = Range,
                CanAttack = false
            };
        }
    }
}