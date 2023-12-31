using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class EntityActionItem : MonoBehaviour, ITooltipProvider {
        [SerializeField] private Image _icon;
        [SerializeField] private BaseButton _button;
        public GeneralUtils.Event OnClick => _button.OnClick;

        public string Tooltip => _config.Name;

        private EntityActionConfig _config;

        public void Initialize(EntityActionConfig config) {
            _config = config;
            _icon.sprite = config.Icon;
        }

        public GameCommand GetCommand(IReadOnlyEntity entity) => _config.GetCommand(entity);
    }
}