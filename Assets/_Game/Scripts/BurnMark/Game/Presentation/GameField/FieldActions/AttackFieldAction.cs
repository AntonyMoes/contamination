using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions {
    public class AttackFieldAction : IFieldAction {
        private readonly IReadOnlyEntity _selectedEntity;
        private readonly IReadOnlyEntity _targetEntity;

        public GameCommand Command { get; }

        public AttackFieldAction(GameCommand attackCommand, IReadOnlyEntity selectedEntity, IReadOnlyEntity targetEntity) {
            Command = attackCommand;
            _selectedEntity = selectedEntity;
            _targetEntity = targetEntity;
        }

        public void DrawPreview(Field field, IFieldActionUIPresenter uiPresenter) {
            field.TileAtPosition(_targetEntity.GetReadOnlyComponent<PositionData>()!.Data.Position).SetState(Tile.State.Attack);
            uiPresenter.ShowAttackPreview(_selectedEntity, _targetEntity);
        }

        public void ClearPreview(Field field, IFieldActionUIPresenter uiPresenter) {
            field.TileAtPosition(_targetEntity.GetReadOnlyComponent<PositionData>()!.Data.Position).SetState(Tile.State.None);
            uiPresenter.HideAttackPreview();
        }
    }
}