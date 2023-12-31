using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class BuildItem : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Image _icon;
        [SerializeField] private BaseButton _buildButton;
        [SerializeField] private GameObject _disabledGroup;

        public Event OnBuild => _buildButton.OnClick;

        private string _name;
        private Cost _cost;
        private int _workToBuild;

        public string Tooltip => $"Build {_name}\n{_cost}\nTurns: {_workToBuild}";

        public void Initialize(Sprite icon, string name, Cost cost, int workToBuild) {
            _icon.sprite = icon;
            _name = name;
            _cost = cost;
            _workToBuild = workToBuild;
        }

        public void SetState(ResourceData resources) {
            _disabledGroup.SetActive(!_cost.CanPay(resources));
        }
        
    }
}