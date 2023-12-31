using _Game.Scripts.BaseUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class BuildQueueItem : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Image _icon;
        [SerializeField] private BaseButton _cancelButton;
        public Event OnCancel => _cancelButton.OnClick;

        [Header("Build group")]
        [SerializeField] private GameObject _buildGroup;
        [SerializeField] private ProgressBar _buildProgress;
        [SerializeField] private TextMeshProUGUI _timer;

        public string Tooltip => $"Cancel building {_name}\n" + (_turnsLeft != 0
            ? $"Turns left: {_turnsLeft}"
            : "Ready to be deployed");

        private string _name;
        private int _turnsLeft;

        public void Initialize(Sprite icon, string name, int turns) {
            _icon.sprite = icon;
            _name = name;
            _buildProgress.Load(0, turns);
            _turnsLeft = turns;
        }

        public void SetState(bool building, int turnsLeft = 0) {
            _buildGroup.SetActive(building);
            if (building) {
                _turnsLeft = turnsLeft;
                _buildProgress.CurrentValue = _buildProgress.MaxValue - turnsLeft;
                _timer.text = turnsLeft.ToString();
            }
        }
    }
}