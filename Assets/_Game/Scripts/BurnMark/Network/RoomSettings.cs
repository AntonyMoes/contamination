using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Configs;
using _Game.Scripts.Lobby;
using LiteNetLib.Utils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Network {
    public class RoomSettings : IRoomSettings<RoomSettings> {
        public const int MaxUsers = 2;

        private Color[] _availableColors;
        private string[] _availableFactions;

        private List<UserSettings> _users = new List<UserSettings>();
        public IReadOnlyList<UserSettings> Users => _users;

        public string Map { get; private set; }
        public string HostId { get; private set; }

        public EUpdateType UpdateType;

        public RoomSettings() { }  // for updates
        private RoomSettings(GameConfig gameConfig) {
            _availableColors = gameConfig.Colors;
            _availableFactions = gameConfig.Factions.Select(f => f.Id).ToArray();
            Map = gameConfig.Maps.First().Id;
        }

        public bool CanJoin(LobbyUser.Data user) {
            return Users.Count < MaxUsers;
        }

        public void Join(LobbyUser.Data user) {
            var firstUnusedColor = _availableColors.First(c => _users.All(u => u.Color != c));
            _users.Add(new UserSettings {
                User = user,
                Color = firstUnusedColor,
                FactionId = _availableFactions.First()
            });

            if (string.IsNullOrEmpty(HostId)) {
                HostId = user.Id;
            }
        }

        public void Leave(string userId) {
            var data = Users.FirstOrDefault(d => d.User.Id == userId);
            if (data == null) {
                return;
            }

            _users.Remove(data);
            if (data.User.Id == HostId) {
                HostId = Users.FirstOrDefault() is { } newHost
                    ? newHost.User.Id
                    : null;
            }
        }

        public bool CanMakeUpdate(LobbyUser.Data user, RoomSettings update) {
            return user.Id == HostId;
        }

        public void MakeUpdate(RoomSettings update) {
            // TODO support updates
            switch (update.UpdateType) {
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool CanStartGame(LobbyUser.Data user) {
            return user.Id == HostId && Users.Count == MaxUsers;
        }

        public void Serialize(NetDataWriter writer) {
            writer.PutArray(_availableColors);
            writer.PutArray(_availableFactions);
            writer.PutArray(_users.ToArray());
            writer.Put(HostId);
            writer.Put(Map);

            writer.Put(UpdateType);
        }

        public void Deserialize(NetDataReader reader) {
            _availableColors = reader.GetColorArray();
            _availableFactions = reader.GetStringArray();
            _users = reader.GetArray<UserSettings>().ToList();
            HostId = reader.GetString();
            Map = reader.GetString();

            UpdateType = reader.GetEnum<EUpdateType>();
        }

        public enum EUpdateType { }

        public class UserSettings : INetSerializable{
            public LobbyUser.Data User;
            public Color Color;
            public string FactionId;

            public void Serialize(NetDataWriter writer) {
                writer.Put(User);
                writer.Put(Color);
                writer.Put(FactionId);
            }

            public void Deserialize(NetDataReader reader) {
                User = reader.Get<LobbyUser.Data>();
                Color = reader.GetColor();
                FactionId = reader.GetString();
            }
        }

        public static RoomSettings CreateDefault(GameConfig gameConfig) {
            return new RoomSettings(gameConfig);
        }
    }
}
