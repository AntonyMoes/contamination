using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public class FakeUnit {
        public static Func<int, Entity> Create(int user, Vector2Int position) {
            return Unit.Create(user, position, new HealthData(), new AttackData(), 0);
        }
    }
}