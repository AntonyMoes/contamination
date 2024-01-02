using UnityEngine;

// using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Terrain {
    [CreateAssetMenu(menuName = Configs.TerrainMenuItem + nameof(TerrainFeatureConfig),
        fileName = nameof(TerrainFeatureConfig))]
    public class TerrainFeatureConfig : Config {
        [SerializeField] private string _name;
        public string Name => _name;

        [SerializeField] private int _additionalMovementCost;
        public int AdditionalMovementCost => _additionalMovementCost;
    }
}