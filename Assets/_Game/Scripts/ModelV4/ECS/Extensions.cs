using System.Collections.Generic;
using System.Linq;

namespace _Game.Scripts.ModelV4.ECS {
    public static class Extensions {
        #region IEnumerable<Entity>

        public static IEnumerable<IReadOnlyEntity> WithComponent<TComponentData>(this IEnumerable<IReadOnlyEntity> entities)
            where TComponentData : struct, ISame<TComponentData> {
            return entities.Where(e => e.GetReadOnlyComponent<TComponentData>() != null);
        }

        public static IEnumerable<IReadOnlyComponent<TComponentData>> GetComponent<TComponentData>(this IEnumerable<IReadOnlyEntity> entities)
            where TComponentData : struct, ISame<TComponentData> {
            return entities
                .Select(e => e.GetReadOnlyComponent<TComponentData>())
                .Where(c => c != null);
        }

        public static IEnumerable<IComponent<TComponentData>> GetModifiableComponent<TComponentData>(this IEnumerable<IEntity> entities)
            where TComponentData : struct, ISame<TComponentData> {
            return entities
                .Select(e => e.GetModifiableComponent<TComponentData>())
                .Where(c => c != null);
        }

        #endregion
    }
}
