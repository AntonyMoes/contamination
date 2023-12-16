using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.TicTacToe.Game.Data;

namespace _Game.Scripts.TicTacToe {
    public static class Utils {
        public static IReadOnlyComponent<MarkData> AtCoordinates(this IReadOnlyDictionary<int, IReadOnlyEntity> entities, int row, int column) {
            return entities.Values
                .GetComponent<PositionData>()
                .First(position => position.Data.Row == row && position.Data.Column == column)
                .ReadOnlyEntity.GetReadOnlyComponent<MarkData>();
        }

        public static SettingsData GetSettings(this IReadOnlyDictionary<int, IReadOnlyEntity> entities) {
            return entities.Values
                .GetComponent<SettingsData>()
                .First()
                .Data;
        }
    }
}
