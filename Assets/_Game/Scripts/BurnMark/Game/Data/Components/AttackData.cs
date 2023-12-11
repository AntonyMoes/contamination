using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct AttackData : ISame<AttackData> {
        public int Damage;
        public int Attacks;
        public int ArmorPiercing;
        public int Range;
        public bool CanAttack;

        public bool IsSame(AttackData other) {
            return Damage == other.Damage
                   && Attacks == other.Attacks
                   && ArmorPiercing == other.ArmorPiercing
                   && Range == other.Range
                   && CanAttack == other.CanAttack;
        }
    }
}