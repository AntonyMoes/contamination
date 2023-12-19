using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions {
    [CreateAssetMenu(menuName = Configs.ActionMenuItem + nameof(DestroyActionConfig), fileName = nameof(DestroyActionConfig))]
    public class DestroyActionConfig : EntityActionConfig {
        public override GameCommand GetCommand(IReadOnlyEntity entity) {
            return new DestroyCommand {
                EntityId = entity.Id
            };
        }
    }
}