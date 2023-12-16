using System;
using _Game.Scripts.BurnMark.Game;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using _Game.Scripts.BurnMark.Game.Pathfinding;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.BurnMark.Game.Presentation.GameUI;
using _Game.Scripts.Fake;
using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.Scheduling;
using UnityEngine;
using Input = _Game.Scripts.BurnMark.Game.Input;
using Terrain = _Game.Scripts.BurnMark.Game.Entities.Terrain;

namespace _Game.Scripts.BurnMark {
    public class FieldTester : MonoBehaviour {
        [SerializeField] private Scheduler _scheduler;
        [SerializeField] private Field _field;
        [SerializeField] private Vector2Int _size;
        [SerializeField] private StartingBase[] _startingBases;
        [SerializeField] private StartingUnit[] _startingUnits;
        [SerializeField] private PlayerUI _playerUI;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private RectTransform _iconsParent;

        private GamePresenter _presenter;
        private ModelV4.Game _game;
        private Input _input;

        private const int Player = 0;

        private void OnEnable() {
            _input = new Input();
            _scheduler.RegisterFrameProcessor(_input);

            var proxy = new ProxyCommandGenerator();
            _game = FakeGame.Create(InitialCommand, proxy);

            GameMechanicsRegistry.RegisterMechanics(_game);

            var accessor = new FieldAccessor(_game.ReadAPI, _game.EventsAPI, new AStar());
            _presenter = new GamePresenter(proxy, Player, _playerUI, _game.EventsAPI, OnGameClosed, CreateFieldPresenter);
            _game.RegisterPresenter(_presenter);

            _game.Start();

            void InitialCommand(GameDataAPI api) {
                api.AddEntity(Game.Entities.Player.Create(Player));

                foreach (var position in _size.EnumeratePositions()) {
                    api.AddEntity(Terrain.Create(position));
                }

                foreach (var startingBase in _startingBases) {
                    api.AddEntity(startingBase.Config.Create(Player, startingBase.Position));
                }

                foreach (var startingUnit in _startingUnits) {
                    api.AddEntity(startingUnit.Config.Create(Player, startingUnit.Position));
                }
            }

            FieldPresenter CreateFieldPresenter(IFieldActionUIPresenter presenter) {
                return new FieldPresenter(_input, _field, accessor, presenter, _size, _uiCamera, _iconsParent);
            }
        }

        private void OnGameClosed() {
            OnDisable();
            OnEnable();
        }

        private void OnDisable() {
            _scheduler.UnregisterFrameProcessor(_input);
            _presenter.Dispose();
            _game.Dispose();
        }

        [Serializable]
        private struct StartingUnit {
            public Vector2Int Position;
            public UnitConfig Config;
        }

        [Serializable]
        private struct StartingBase {
            public Vector2Int Position;
            public BaseConfig Config;
        }
    }
}