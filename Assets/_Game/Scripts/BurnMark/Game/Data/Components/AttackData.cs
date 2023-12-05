using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Data.Components {
    public struct AttackData : ISame<AttackData> {
        public int Damage;
        public int Attacks;
        public int ArmorPiercing;
        public int Range;
        public bool CanAttack;

        public bool IsSame(ISame<AttackData> other) {
            var o = other.Get();
            return Damage == o.Damage
                   && Attacks == o.Attacks
                   && ArmorPiercing == o.ArmorPiercing
                   && Range == o.Range
                   && CanAttack == o.CanAttack;
        }
    }
}