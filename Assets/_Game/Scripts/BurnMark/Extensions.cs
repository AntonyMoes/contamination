using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;

namespace _Game.Scripts.BurnMark {
    public static class Extensions {
        #region IReadOnlyEntity

        [CanBeNull]
        public static FieldEntityConfig TryGetFieldEntityConfig(this IReadOnlyEntity entity) {
            return entity.GetReadOnlyComponent<UnitData>() is { } unitData
                ? (FieldEntityConfig) unitData.Data.Config
                : entity.GetReadOnlyComponent<FieldObjectData>() is { } fieldObjectData
                    ? fieldObjectData.Data.Config
                    : null;
        }

        #endregion
    }
}