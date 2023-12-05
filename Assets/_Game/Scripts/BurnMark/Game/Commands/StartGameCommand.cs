using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class StartGameCommand : GameCommand {
        public int[] Players;
        public MapData MapData;
        
        protected override void SerializeContents(NetDataWriter writer) {
            writer.PutArray(Players);
            writer.Put(JsonUtility.ToJson(MapData));
        }

        protected override void DeserializeContents(NetDataReader reader) {
            Players = reader.GetIntArray();
            MapData = JsonUtility.FromJson<MapData>(reader.GetString());
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            for (var i = 0; i < Players.Length; i++) {
                var player = Players[i];
                var basePosition = MapData.PlayerBases[i];

                api.AddEntity(Player.Create(player));
                api.AddEntity(Base.Create(player, basePosition));
            }
        }
    }
}