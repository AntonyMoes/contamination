using _Game.Scripts.BurnMark.Game.Commands;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions {
    public interface IFieldAction {
        public void DrawPreview(Field field, IFieldActionUIPresenter uiPresenter);
        public void ClearPreview(Field field, IFieldActionUIPresenter uiPresenter);
        public GameCommand Command { get; }
    }
}