using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.ECS;
using LiteNetLib.Utils;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class TicTacToeInitialCommand : GameCommand {
        public int Size;
        public int[] Players;
        public MarkComponent.EMark[] Marks;

        protected override Action<GameDataAPI> PerformReversibleDoOnData(GameDataAPI api) {
            var ids = new List<int> {
                api.AddEntity(id => {
                    Func<Entity, IComponent> settingsComponent = entity => new SettingsComponent(entity) {
                        Data = new SettingsComponent.SettingsData {
                            Size = Size,
                            Players = Players,
                            Marks = Marks
                        }
                    };
                    return new Entity(id, settingsComponent);
                })
            };

            for (var row = 0; row < Size; row++) {
                for (var column = 0; column < Size; column++) {
                    var currentRow = row;
                    var currentColumn = column;
                    ids.Add(api.AddEntity(id => {
                        Func<Entity, IComponent>  markComponent = entity => new MarkComponent(entity) {
                            Data = new MarkComponent.MarkData {
                                Mark = MarkComponent.EMark.None
                            }
                        };
                        Func<Entity, IComponent> positionComponent = entity => new PositionComponent(entity) {
                            Data = new PositionComponent.PositionData {
                                Row = currentRow,
                                Column = currentColumn
                            }
                        };
                        return new Entity(id, markComponent, positionComponent);
                    }));
                }
            }

            return api => {
                ids.Reverse();
                foreach (var id in ids) {
                    api.RemoveEntity(id);
                }
            };
        }

        public override string SerializeContents() {
            return $"{Size};{Players.Length};"
                             + Players.Select(id => $"{id};").Aggregate((str1, str2) => str1 + str2)
                             + Marks.Select(mark => $"{mark};").Aggregate((str1, str2) => str1 + str2);
        }

        public override void DeserializeContents(string contents) {
            var args = contents.Split(';');
            Size = int.Parse(args[0]);
            var arraySize = int.Parse(args[1]);
            const int startIdx = 2;
            Players = Enumerable.Range(startIdx, arraySize)
                .Select(i => int.Parse(args[i]))
                .ToArray();
            Marks = Enumerable.Range(startIdx + arraySize, arraySize)
                .Select(i => {
                    Enum.TryParse<MarkComponent.EMark>(args[i], out var mark);
                    return mark;
                }) 
                .ToArray();
        }
    }
}
