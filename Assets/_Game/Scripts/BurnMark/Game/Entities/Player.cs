using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Player {
        public static Func<int, Entity> Create(int user, Color color) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var playerComponent = Entity.Add(new PlayerData {
                Color = color
            });
            var resourceComponent = Entity.Add(new ResourceData());
            return id => new Entity(id, ownerComponent, playerComponent, resourceComponent);
        }

        public static bool IsPlayer(IReadOnlyEntity entity) {
            return entity.GetReadOnlyComponent<OwnerData>() != null
                   && entity.GetReadOnlyComponent<ResourceData>() != null;
        }
    }
}