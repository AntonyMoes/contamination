using System.Linq;
using _Game.Scripts.BurnMark.Game;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Configs;
using _Game.Scripts.BurnMark.Game.Pathfinding;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.BurnMark.Game.Presentation.GameUI;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.Scheduling;
using UnityEngine;
using Input = _Game.Scripts.BurnMark.Game.Input;

namespace _Game.Scripts.BurnMark {
    public class FieldTester : MonoBehaviour {
        [SerializeField] private Scheduler _scheduler;
        [SerializeField] private Field _field;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private string _map;
        [SerializeField] private string _faction;
        [SerializeField] private PlayerUI _playerUI;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private RectTransform _iconsParent;

        private GamePresenter _presenter;
        private ModelV4.Game _game;
        private Input _input;

        private const int Player = 12;
        private const int Player2 = 30;

        private void OnEnable() {
            _input = new Input();
            _scheduler.RegisterFrameProcessor(_input);

            var initialCommand = new StartGameCommand {
                Players = new [] { Player, Player2 },
                Factions = new [] { _faction, _faction },
                Colors = new [] { _gameConfig.Colors.First(), _gameConfig.Colors.Skip(1).First() },
                Map = _map
            };
            initialCommand.SetConfig(_gameConfig);

            var proxy = new LocalProxyCommandGenerator();
            _game = ModelV4.Game.CreateLocal(initialCommand, new(int, string, ICommandGenerator)[] {
                (Player, nameof(Player), proxy.Get(Player)),
                (Player2, nameof(Player2), proxy.Get(Player2))
            });

            var accessor = new FieldAccessor(_game.ReadAPI, _game.EventsAPI, new AStar());
            GameMechanicsRegistry.RegisterMechanics(_game, accessor);
            _presenter = new GamePresenter(proxy, _playerUI, _game.EventsAPI, OnGameClosed, CreateFieldPresenter, initialCommand.Players);
            _game.RegisterPresenter(_presenter);

            _game.Start();

            FieldPresenter CreateFieldPresenter(IFieldActionUIPresenter presenter) {
                return new FieldPresenter(_input, _field, accessor, presenter, _uiCamera, _iconsParent, _scheduler);
            }
        }

        private void OnGameClosed() {
            OnDisable();
            OnEnable();
        }

        private void OnDisable() {
            _scheduler.UnregisterFrameProcessor(_input);
            _game.Dispose();
            _presenter.Dispose();
        }
    }
}