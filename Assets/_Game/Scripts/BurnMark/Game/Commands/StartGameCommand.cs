using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Configs;
using _Game.Scripts.ModelV4;
using LiteNetLib.Utils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public class StartGameCommand : GameCommand {
        public int[] Players;
        public string[] Factions;
        public Color[] Colors;
        public string Map;

        private GameConfig _gameConfig;

        protected override void SerializeContents(NetDataWriter writer) {
            writer.PutArray(Players);
            writer.PutArray(Factions);
            writer.PutArray(Colors);
            writer.Put(Map);
        }

        protected override void DeserializeContents(NetDataReader reader) {
            Players = reader.GetIntArray();
            Factions = reader.GetStringArray();
            Colors = reader.GetColorArray();
            Map = reader.GetString();
        }

        public void SetConfig(GameConfig gameConfig) {
            _gameConfig = gameConfig;
        }

        protected override void PerformDoOnAPI(GameDataAPI api) {
            if (_gameConfig == null) {
                throw new Exception("No GameConfig was set");
            }

            var map = _gameConfig.Maps.First(m => m.Id == Map);
            api.AddEntity(Entities.Map.Create(map.Terrain.Size()));

            var terrain = map.Terrain;
            foreach (var position in terrain.Size().EnumeratePositions()) {
                if (terrain[position.x, position.y] is {} terrainData) {
                    api.AddEntity(Entities.Terrain.Create(position, terrainData));
                }
            }

            for (var i = 0; i < Players.Length; i++) {
                var player = Players[i];
                var faction = _gameConfig.Factions.First(f => f.Id == Factions[i]);
                var color = Colors[i];
                var startingPosition = map.StartingPoints[i];

                api.AddEntity(Entities.Player.Create(player, color));
                api.AddEntity(faction.Base.Create(player, startingPosition));
            }
        }
    }
}