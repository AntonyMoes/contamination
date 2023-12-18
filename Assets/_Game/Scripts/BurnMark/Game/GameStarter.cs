using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.BurnMark.Game.Presentation.GameUI;
using _Game.Scripts.BurnMark.Network;
using _Game.Scripts.Lobby;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.Network;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game {
    public static class GameStarter {
        private static FieldPresenter _fieldPresenter;

        public static ModelV4.Game StartClientGame(GameConfigurationMessage message, IPeer serverPeer,
            PlayerUI playerUI, Action onClientClosedGame) {
            var proxy = new ProxyCommandGenerator();
            var game = ModelV4.Game.StartClient(message, serverPeer, proxy);
            GamePresenter presenter = null;
            presenter = new GamePresenter(proxy, playerUI, game.EventsAPI, OnClientClosedGame, null, null/*TODO*/);
            game.RegisterPresenter(presenter);

            GameMechanicsRegistry.RegisterMechanics(game, null);  // TODO

            return game;

            void OnClientClosedGame() {
                presenter.Dispose();
                onClientClosedGame?.Invoke();
            }
        }

        public static ModelV4.Game StartServerGame(IReadOnlyCollection<LobbyUser> users, Action<LobbyUser, GameConfigurationMessage> sendMessage) {
            var orderedUsers = users.ToArray(); //

            for (var i = 0; i < RoomSettings.MaxUsers; i++) {
                var user = orderedUsers[i];
                sendMessage(user, CreateConfigurationForUser(orderedUsers, IdFromIndex(i)));
            }

            var game = ModelV4.Game.StartStandaloneServer(CreateConfigurationForUser(orderedUsers), users.Select(u => u.Peer));

            GameMechanicsRegistry.RegisterMechanics(game, null);  // TODO

            return game;
        }

        private static GameConfigurationMessage CreateConfigurationForUser(IEnumerable<LobbyUser> orderedUsers, int id = 0) {
            var userIds = Enumerable.Range(0, RoomSettings.MaxUsers).Select(IdFromIndex).ToArray();
            return new GameConfigurationMessage {
                InitialCommand = new OldStartGameCommand {
                    Players = userIds,
                    MapData = new MapData {
                        PlayerBases = new [] {
                            Vector2Int.zero,
                            Vector2Int.one
                        }
                    }
                },
                CurrenUser = id,
                UserSequence = userIds,
                UserNames = orderedUsers.Select(u => u.Name).ToArray(),
            };
        }

        private static int IdFromIndex(int index) => index + 1;
    }
}