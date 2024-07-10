using System;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data {
    [Serializable]
    public class Cost {
        [SerializeField] private Resources _resources;
        public Resources Resources => _resources;

        public Cost Refund() {
            return new Cost {
                _resources = _resources.Inverted()
            };
        }

        public override string ToString() => _resources.ToString();
    }
}