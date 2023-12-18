using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.ModelV4.ECS;
using JetBrains.Annotations;
using UnityEngine;

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

        #region 2DArray

        public static Vector2Int Size<T>(this T[,] array) {
            return new Vector2Int(array.GetLength(0), array.GetLength(1));
        }

        #endregion
    }
}