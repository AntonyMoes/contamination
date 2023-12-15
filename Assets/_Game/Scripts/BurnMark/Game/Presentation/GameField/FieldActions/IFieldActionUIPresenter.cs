using _Game.Scripts.ModelV4.ECS;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions {
    public interface IFieldActionUIPresenter {
        public void ShowAttackPreview(IReadOnlyEntity attacker, IReadOnlyEntity target);
        public void HideAttackPreview();
    }
}