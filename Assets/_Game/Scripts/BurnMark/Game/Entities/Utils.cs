using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Entities {
    public static class Utils {
        public static int? GetOwnerId(this IReadOnlyEntity entity) {
            return entity.GetReadOnlyComponent<OwnerData>()!.Data.Owner;
        }
    }
}