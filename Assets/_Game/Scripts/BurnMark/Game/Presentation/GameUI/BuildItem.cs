using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Data;
using _Game.Scripts.BurnMark.Game.Data.Components;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class BuildItem : MonoBehaviour {
        [SerializeField] private Image _icon;
        [SerializeField] private BaseButton _buildButton;
        [SerializeField] private GameObject _disabledGroup;

        public Event OnBuild => _buildButton.OnClick;

        private Cost _cost;

        public void Initialize(Sprite icon, Cost cost) {
            _icon.sprite = icon;
            _cost = cost;
        }

        public void SetState(ResourceData resources) {
            _disabledGroup.SetActive(!_cost.CanPay(resources));
        }
    }
}