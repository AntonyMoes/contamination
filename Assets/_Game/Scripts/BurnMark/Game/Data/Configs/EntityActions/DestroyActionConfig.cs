using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.ModelV4.ECS;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions {
    public class DestroyActionConfig : EntityActionConfig {
        public override GameCommand GetCommand(IReadOnlyEntity entity) {
            return new DestroyCommand {
                EntityId = entity.Id
            };
        }

#if UNITY_EDITOR
        [MenuItem(Configs.ActionMenuItem + nameof(DestroyActionConfig), false)]
        public static void Create() => Configs.Create<DestroyActionConfig>();
#endif
    }
}