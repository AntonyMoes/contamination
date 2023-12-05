using System;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public class Player {
        public static Func<int, Entity> Create(int user) {
            var ownerComponent = Entity.Add(new OwnerData {
                Owner = user
            });
            var resourceComponent = Entity.Add(new ResourceData());
            return id => new Entity(id, ownerComponent, resourceComponent);
        }

        public static bool IsPlayer(IReadOnlyEntity entity) {
            return entity.GetReadOnlyComponent<OwnerData>() != null
                   && entity.GetReadOnlyComponent<ResourceData>() != null;
        }
    }
}