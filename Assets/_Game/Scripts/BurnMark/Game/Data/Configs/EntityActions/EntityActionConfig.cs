using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions {
    public abstract class EntityActionConfig : Config {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private Sprite _icon;
        public Sprite Icon => _icon;

        public abstract GameCommand GetCommand(IReadOnlyEntity entity);
    }
}