using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.ModelV4.NetTicTacToeExample.Data;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public static class Utils {
        public static IReadOnlyComponent<MarkData> AtCoordinates(this IEnumerable<IReadOnlyEntity> entities, int row, int column) {
            return entities
                .GetComponent<PositionData>()
                .First(position => position.Data.Row == row && position.Data.Column == column)
                .Entity.GetReadOnlyComponent<MarkData>();
        }

        public static SettingsData GetSettings(this IEnumerable<IReadOnlyEntity> entities) {
            return entities
                .GetComponent<SettingsData>()
                .First()
                .Data;
        }
    }
}
