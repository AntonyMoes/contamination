using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Configs;
using _Game.Scripts.BurnMark.Game.Pathfinding;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.BurnMark.Game.Presentation.GameUI;
using _Game.Scripts.BurnMark.Network;
using _Game.Scripts.Lobby;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.Network;
using _Game.Scripts.Scheduling;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game {
    public static class GameStarter {
        public static ModelV4.Game StartClientGame(GameConfig gameConfig, GameConfigurationMessage message,
            IPeer serverPeer, PlayerUI playerUI, Input input, Field field, Camera uiCamera, RectTransform iconsParent,
            IScheduler scheduler, PointerRaycastProvider pointerRaycastProvider, Action onClientClosedGame) {
            ((StartGameCommand) message.InitialCommand).SetConfig(gameConfig);

            var proxy = new ProxyCommandGenerator();
            var game = ModelV4.Game.StartClient(message, serverPeer, proxy);
            var accessor = new FieldAccessor(game.ReadAPI, game.EventsAPI, new AStar());
            GameMechanicsRegistry.RegisterMechanics(game, accessor);

            GamePresenter presenter = null;
            presenter = new GamePresenter(proxy, playerUI, uiCamera, game.EventsAPI, scheduler, pointerRaycastProvider, OnClientClosedGame, CreateFieldPresenter, message.Players);
            game.RegisterPresenter(presenter);

            return game;

            FieldPresenter CreateFieldPresenter(IFieldActionUIPresenter presenter) {
                return new FieldPresenter(input, field, accessor, presenter, uiCamera, iconsParent, scheduler);
            }

            void OnClientClosedGame() {
                presenter.Dispose();
                onClientClosedGame?.Invoke();
            }
        }

        public static ModelV4.Game StartServerGame(RoomSettings roomSettings, IReadOnlyCollection<LobbyUser> users,
            Action<LobbyUser, GameConfigurationMessage> sendMessage) {
            var orderedUsers = users.ToArray(); //
            var userIds = Enumerable.Range(0, orderedUsers.Length).Select(IdFromIndex).ToArray();

            for (var i = 0; i < orderedUsers.Length; i++) {
                var user = orderedUsers[i];
                var userId = userIds[i];
                sendMessage(user, CreateConfigurationForUser(roomSettings, orderedUsers, userIds, userId));
            }

            var game = ModelV4.Game.StartStandaloneServer(CreateConfigurationForUser(roomSettings, orderedUsers, userIds), users.Select(u => u.Peer));
            var accessor = new FieldAccessor(game.ReadAPI, game.EventsAPI, new AStar());
            GameMechanicsRegistry.RegisterMechanics(game, accessor);

            return game;
        }

        private static GameConfigurationMessage CreateConfigurationForUser(RoomSettings roomSettings,
            IEnumerable<LobbyUser> orderedUsers, int[] userIds, int userId = 0) {
            return new GameConfigurationMessage {
                InitialCommand = new StartGameCommand {
                    Players = userIds,
                    Colors = roomSettings.Users.Select(s => s.Color).ToArray(),
                    Factions = roomSettings.Users.Select(s => s.FactionId).ToArray(),
                    Map = roomSettings.Map
                },
                CurrenUser = userId,
                Players = userIds,
                PlayerNames = orderedUsers.Select(u => u.Name).ToArray(),
            };
        }

        private static int IdFromIndex(int index) => index + 1;
    }
}