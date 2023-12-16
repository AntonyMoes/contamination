using _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Entities {
    public abstract class FieldEntityConfig  : Config {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        [SerializeField] private EntityActionConfig[] _actions;
        public EntityActionConfig[] Actions => _actions;
    }
}