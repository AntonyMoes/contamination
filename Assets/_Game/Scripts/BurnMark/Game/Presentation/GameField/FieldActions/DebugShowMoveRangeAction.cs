using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions {
    public class DebugShowMoveRangeAction : IFieldAction {
        private readonly IReadOnlyEntity _entity;
        private readonly Vector2Int[] _tiles;

        public GameCommand Command => null;

        public DebugShowMoveRangeAction(Vector2Int[] tiles) {
            _tiles = tiles;
        }
        
        public void DrawPreview(Field field, IFieldActionUIPresenter uiPresenter) {
            foreach (var tile in _tiles) {
                field.TileAtPosition(tile).SetState(Tile.State.Highlighted);
            }
        }

        public void ClearPreview(Field field, IFieldActionUIPresenter uiPresenter) {
            foreach (var tile in _tiles) {
                field.TileAtPosition(tile).SetState(Tile.State.None);
            }
        }
    }
}