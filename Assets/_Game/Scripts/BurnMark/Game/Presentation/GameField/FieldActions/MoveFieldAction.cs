using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions {
    public class MoveFieldAction : IFieldAction {
        private readonly IReadOnlyEntity _entity;
        private readonly Vector2Int[] _path;

        public GameCommand Command { get; }

        public MoveFieldAction(GameCommand command, IReadOnlyEntity entity, Vector2Int[] path) {
            Command = command;
            _entity = entity;
            _path = path;
        }

        public void DrawPreview(Field field, IFieldActionUIPresenter uiPresenter) {
            var moveDistance = _entity.GetReadOnlyComponent<MoveData>()!.Data.RemainingDistance;
            for (var i = 1; i < _path.Length; i++) {
                var position = _path[i];
                field.TileAtPosition(position).SetState(i > moveDistance ? Tile.State.Forbidden : Tile.State.Selected);
            }
        }

        public void ClearPreview(Field field, IFieldActionUIPresenter uiPresenter) {
            for (var i = 1; i < _path.Length; i++) {
                var position = _path[i];
                field.TileAtPosition(position).SetState(Tile.State.None);
            }
        }
    }
}