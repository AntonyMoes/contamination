using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class SettingsComponent : Component<SettingsComponent.SettingsData> {
        public struct SettingsData : ISame<SettingsData> {
            public int Size;
            public int[] Players;
            public MarkComponent.EMark[] Marks;

            public Dictionary<int, MarkComponent.EMark> MarkPerPlayer => Players.ZipToDictionary(Marks);
            public Dictionary<MarkComponent.EMark, int> PlayerPerMark => Marks.ZipToDictionary(Players);

            public bool IsSame(ISame<SettingsData> other) {
                return Size == other.Get().Size;
            }
        }

        public SettingsComponent(Entity correspondingEntity) : base(correspondingEntity) { }
    }
}
