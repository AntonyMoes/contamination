using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.Commands;
using _Game.Scripts.TicTacToe.Game.Commands;
using _Game.Scripts.TicTacToe.Game.Data;
using _Game.Scripts.TicTacToe.Game.Presentation;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.TicTacToe.Game {
    public class TicTacToeInteractor : MonoBehaviour, ICommandGenerator, ICommandPresenter {
        [SerializeField] private Tile _tilePrefab;
        [SerializeField] private RectTransform _container;
        [SerializeField] private GridLayoutGroup _layout;

        private readonly List<Tile> _tiles = new List<Tile>();
        private readonly Action<GameCommand> _onCommandGenerated;
        private int _currentUser;
        private GameDataReadAPI _readApi;


        public TicTacToeInteractor() {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
        }

        private void InitializeField(TicTacToeInitialCommand initCommand) {
            foreach (var tile in _tiles) {
                tile.OnClick.RemoveAllListeners();
                Destroy(tile.gameObject);
            }
            _tiles.Clear();

            _container.sizeDelta = _layout.cellSize * initCommand.Size + _layout.spacing * (initCommand.Size - 1);

            for (var i = 0; i < initCommand.Size; i++) {
                for (var j = 0; j < initCommand.Size; j++) {
                    var tile = Instantiate(_tilePrefab, _layout.transform);
                    tile.SetMark(MarkData.EMark.None);
                    tile.OnClick.AddListener(() => OnTileClick(tile));
                    _tiles.Add(tile);
                }
            }
        }

        private void MarkTile(MarkCommand markCommand) {
            var entity = _readApi.Entities[markCommand.EntityId];
            var position = entity.GetReadOnlyComponent<PositionData>()!.Data;
            var settings = _readApi.Entities.GetSettings();
            _tiles[position.Row * settings.Size + position.Column].SetMark(markCommand.Mark);
        }

        public void SetCurrentUser(int user) {
            _currentUser = user;
        }

        private void OnTileClick(Tile tile) {
            if (!(_readApi.CurrentUser is { } current) || current.Id != _currentUser) {
                return;
            }

            var tileIndex = _tiles.IndexOf(tile);
            var (row, column) = TileIndexToRowAndColumn(_readApi.Entities.GetSettings().Size, tileIndex);
            var markComponent = _readApi.Entities.AtCoordinates(row, column);
            if (markComponent.Data.Mark == MarkData.EMark.None) {
                var settings = _readApi.Entities.GetSettings();
                _onCommandGenerated(new MarkCommand {
                    EntityId = markComponent.ReadOnlyEntity.Id,
                    Mark = settings.MarkPerPlayer[_currentUser]
                });
            }
        }

        private static (int, int) TileIndexToRowAndColumn(int size, int tileIndex) {
            return (tileIndex / size, tileIndex % size);
        }

        public Event<GameCommand> OnCommandGenerated { get; }

        public void SetReadAPI(IGameReadAPI api) {
            _readApi = (GameDataReadAPI) api;
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            return generatedCommand switch {
                TicTacToeInitialCommand initCommand => new SyncProcess(() => InitializeField(initCommand)),
                MarkCommand markCommand => new SyncProcess(() => MarkTile(markCommand)),
                _ => new DummyProcess()
            };
        }
    }
}
