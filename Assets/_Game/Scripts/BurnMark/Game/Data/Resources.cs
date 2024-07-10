using System;
using System.Linq;

namespace _Game.Scripts.BurnMark.Game.Data {
    [Serializable]
    public struct Resources {
        public int Money;
        public int Metal;

        public bool IsSame(Resources other) {
            return Money == other.Money
                   && Metal == other.Metal;
        }

        public bool EnoughFor(Resources cost) {
            return Money >= cost.Money && Metal >= cost.Metal;
        }

        public Resources Add(Resources add) {
            return new Resources {
                Money = Money + add.Money,
                Metal = Metal + add.Metal
            };
        }

        public Resources Subtract(Resources subtract) {
            return new Resources {
                Money = Money - subtract.Money,
                Metal = Metal - subtract.Metal
            };
        }

        public Resources Inverted() {
            return new Resources {
                Money = -Money,
                Metal = -Metal
            };
        }

        public override string ToString() {
            var parts = new[] {
                Money != 0 ? $"Money: {Money}" : "",
                Metal != 0 ? $"Metal: {Metal}" : ""
            };
            return string.Join(", ", parts.Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}