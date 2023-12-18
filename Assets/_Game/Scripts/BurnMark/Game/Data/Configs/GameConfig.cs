using _Game.Scripts.BurnMark.Game.Data.Configs.Map;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    public class GameConfig : Config {
        [SerializeField] private MapConfig[] _maps;
        public MapConfig[] Maps => _maps;

        [SerializeField] private FactionConfig[] _factions;
        public FactionConfig[] Factions => _factions;

#if UNITY_EDITOR
        [MenuItem(Configs.MenuItem + nameof(GameConfig), false)]
        public static void Create() => Configs.Create<GameConfig>();
#endif
    }
}