using _Game.Scripts.BurnMark.Game.Data.Configs.Map;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    [CreateAssetMenu(menuName = Configs.MenuItem + nameof(GameConfig), fileName = nameof(GameConfig))]
    public class GameConfig : Config {
        [SerializeField] private MapConfig[] _maps;
        public MapConfig[] Maps => _maps;

        [SerializeField] private FactionConfig[] _factions;
        public FactionConfig[] Factions => _factions;

        [SerializeField] private Color[] _colors;
        public Color[] Colors => _colors;
    }
}