using System;
using System.Linq;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.TicTacToe.Data;

namespace _Game.Scripts.TicTacToe.Commands {
    public class TicTacToeInitialCommand : TicTacToeCommand {
        public int Size;
        public int[] Players;
        public MarkData.EMark[] Marks;

        protected override void PerformDoOnAPI(GameDataAPI api) {
            api.AddEntity(id => new Entity(id, Entity.Add(new SettingsData {
                Size = Size,
                Players = Players,
                Marks = Marks
            })));

            for (var row = 0; row < Size; row++) {
                for (var column = 0; column < Size; column++) {
                    var currentRow = row;
                    var currentColumn = column;
                    api.AddEntity(id => {
                        var mark = Entity.Add(new MarkData {
                            Mark = MarkData.EMark.None
                        });
                        var position = Entity.Add(new PositionData {
                            Row = currentRow,
                            Column = currentColumn
                        });
                        return new Entity(id, mark, position);
                    });
                }
            }
        }

        protected override string SerializeContents() {
            return $"{Size};{Players.Length};"
                             + Players.Select(id => $"{id};").Aggregate((str1, str2) => str1 + str2)
                             + Marks.Select(mark => $"{mark};").Aggregate((str1, str2) => str1 + str2);
        }

        protected override void DeserializeContents(string contents) {
            var args = contents.Split(';');
            Size = int.Parse(args[0]);
            var arraySize = int.Parse(args[1]);
            const int startIdx = 2;
            Players = Enumerable.Range(startIdx, arraySize)
                .Select(i => int.Parse(args[i]))
                .ToArray();
            Marks = Enumerable.Range(startIdx + arraySize, arraySize)
                .Select(i => {
                    Enum.TryParse<MarkData.EMark>(args[i], out var mark);
                    return mark;
                }) 
                .ToArray();
        }
    }
}
