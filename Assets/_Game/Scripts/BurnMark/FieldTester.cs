using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Presentation;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.Fake;
using _Game.Scripts.ModelV4;
using UnityEngine;
using Terrain = _Game.Scripts.BurnMark.Game.Entities.Terrain;

namespace _Game.Scripts.BurnMark {
    public class FieldTester : MonoBehaviour {
        [SerializeField] private Field _field;
        [SerializeField] private Vector2Int _size;
        [SerializeField] private Vector2Int[] _baseLocations;
        [SerializeField] private Vector2Int[] _unitLocations;

        private GameFieldPresenter _presenter;
        private ModelV4.Game _game;

        private void OnEnable() {
            _presenter = new GameFieldPresenter();

            _game = FakeGame.Create(InitialCommand, _presenter);
            _game.RegisterPresenter(_presenter);

            var accessor = new FieldAccessor(_game.ReadAPI, _game.EventsAPI);

            _game.Start();
            _presenter.Start(_field, accessor, _size, _baseLocations, _unitLocations);

            void InitialCommand(GameDataAPI api) {
                foreach (var position in _size.EnumeratePositions()) {
                    api.AddEntity(Terrain.Create(position));
                }

                foreach (var baseLocation in _baseLocations) {
                    api.AddEntity(Base.Create(0, baseLocation));
                }

                foreach (var unitLocation in _unitLocations) {
                    api.AddEntity(FakeUnit.Create(0, unitLocation));
                }
            }
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                _presenter.OnSelectClick();
            }
            if (Input.GetMouseButtonDown(1)) {
                _presenter.OnActionClick();
            }
        }

        private void OnDisable() {
            _game.Dispose();
            _presenter.Clear();
        }
    }
}