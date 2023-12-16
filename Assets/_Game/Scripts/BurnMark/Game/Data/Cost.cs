using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data {
    [Serializable]
    public class Cost {
        [SerializeField] private int _money;
        public int Money => _money;

        [SerializeField] private int _metal;
        public int Metal => _metal;

        public bool CanPay(ResourceData resourceData) {
            return resourceData.Money >= _money
                   && resourceData.Metal >= _metal;
        }

        public Cost Refund() {
            return new Cost {
                _money = -_money,
                _metal = -_metal
            };
        }
    }
}