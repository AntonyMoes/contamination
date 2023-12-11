using _Game.Scripts.BurnMark.Game;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Pathfinding;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
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
        [SerializeField] private Vector2Int[] _baseLocations;
        [SerializeField] private Vector2Int[] _unitLocations;
        [SerializeField] private PlayerUI _playerUI;

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
            var fieldPresenter = new GameFieldPresenter(_input, _field, accessor, _size, _baseLocations, _unitLocations);
            _presenter = new GamePresenter(proxy, Player, _playerUI, _game.EventsAPI, OnGameClosed, fieldPresenter);
            _game.RegisterPresenter(_presenter);

            _game.Start();

            void InitialCommand(GameDataAPI api) {
                api.AddEntity(Game.Entities.Player.Create(Player));

                foreach (var position in _size.EnumeratePositions()) {
                    api.AddEntity(Terrain.Create(position));
                }

                foreach (var baseLocation in _baseLocations) {
                    api.AddEntity(Base.Create(Player, baseLocation));
                }

                foreach (var unitLocation in _unitLocations) {
                    api.AddEntity(FakeUnit.Create(Player, unitLocation));
                }
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
    }
}