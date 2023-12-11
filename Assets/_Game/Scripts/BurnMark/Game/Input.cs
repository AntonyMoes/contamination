using _Game.Scripts.Scheduling;
using GeneralUtils;

namespace _Game.Scripts.BurnMark.Game {
    public class Input : IFrameProcessor {
        private readonly UpdatedValue<bool> _selectionButton = new UpdatedValue<bool>();
        public IUpdatedValue<bool> SelectionButton => _selectionButton;
        private readonly UpdatedValue<bool> _actionButton = new UpdatedValue<bool>();
        public  IUpdatedValue<bool> ActionButton => _actionButton;

        private const int SelectionMouseButton = 0;
        private const int ActionMouseButton = 1;

        public void ProcessFrame(float deltaTime) {
            if (UnityEngine.Input.GetMouseButtonDown(SelectionMouseButton)) {
                _selectionButton.Value = true;
            }

            if (UnityEngine.Input.GetMouseButtonUp(SelectionMouseButton)) {
                _selectionButton.Value = false;
            }

            if (UnityEngine.Input.GetMouseButtonDown(ActionMouseButton)) {
                _actionButton.Value = true;
            }

            if (UnityEngine.Input.GetMouseButtonUp(ActionMouseButton)) {
                _actionButton.Value = false;
            }
        }
    }
}