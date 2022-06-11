using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public static class Utils {
        public static IReadOnlyComponent<MarkComponent.MarkData> AtCoordinates(this IEnumerable<IReadOnlyEntity> entities, int row, int column) {
            return entities.First(entity => entity.GetReadOnlyComponent<PositionComponent, PositionComponent.PositionData>() is
                                         { } position
                                     && position.Data.Row == row && position.Data.Column == column)
                .GetReadOnlyComponent<MarkComponent, MarkComponent.MarkData>();
        }

        public static SettingsComponent.SettingsData GetSettings(this IEnumerable<IReadOnlyEntity> entities) {
            return entities
                .Select(entity => entity.GetReadOnlyComponent<SettingsComponent, SettingsComponent.SettingsData>())
                .First(s => s != null)
                .Data;
        }
    }
}
