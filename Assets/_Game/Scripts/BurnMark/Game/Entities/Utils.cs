using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Utils {
        public static int? GetOwnerId(this IReadOnlyEntity entity) {
            return entity.GetReadOnlyComponent<OwnerData>()!.Data.Owner;
        }

        [CanBeNull]
        public static IComponent<T> GetInOwner<T>(this IReadOnlyEntity entity, GameDataAPI api) where T : struct, ISame<T> {
            var owner = entity.GetOwnerId();
            return GetInOwner<T>(owner, api);
        }

        [CanBeNull]
        public static IComponent<T> GetInOwner<T>(int? owner, GameDataAPI api) where T : struct, ISame<T> {
            if (!(owner is { } ownerId)) {
                return null;
            }

            return api.ModifiableEntities.Values
                .GetModifiableComponent<T>()
                .First(component => component.Entity.GetOwnerId() == ownerId);
        }

        [CanBeNull]
        public static IReadOnlyComponent<T> GetInReadOnlyOwner<T>(this IReadOnlyEntity entity, GameDataReadAPI api) where T : struct, ISame<T> {
            var owner = entity.GetOwnerId();
            return GetInReadOnlyOwner<T>(owner, api);
        }

        [CanBeNull]
        public static IReadOnlyComponent<T> GetInReadOnlyOwner<T>(int? owner, GameDataReadAPI api) where T : struct, ISame<T> {
            if (!(owner is { } ownerId)) {
                return null;
            }

            return api.Entities.Values
                .GetComponent<T>()
                .First(component => component.ReadOnlyEntity.GetOwnerId() == ownerId);
        }
    }
}