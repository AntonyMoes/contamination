using System.Collections.Generic;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample.Data {
    public struct SettingsData : ISame<SettingsData> {
        public int Size;
        public int[] Players;
        public MarkData.EMark[] Marks;

        public Dictionary<int, MarkData.EMark> MarkPerPlayer => Players.ZipToDictionary(Marks);
        public Dictionary<MarkData.EMark, int> PlayerPerMark => Marks.ZipToDictionary(Players);

        public bool IsSame(ISame<SettingsData> other) {
            return Size == other.Get().Size;
        }
    }
}
