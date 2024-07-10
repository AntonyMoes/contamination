using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Terrain {
    [CreateAssetMenu(menuName = Configs.TerrainMenuItem + nameof(TerrainResourceFeatureConfig), fileName = nameof(TerrainResourceFeatureConfig))]
    public class TerrainResourceFeatureConfig : TerrainFeatureConfig {
        [SerializeField] private ResourceGainData _resourceGainData;

        public override string Tooltip => $"{base.Tooltip}. Gain per turn:\n{_resourceGainData.Resources}";

        public override IEnumerable<Func<Entity, IComponent>> GetAdditionalComponents() {
            return new [] {
                Entity.Add(_resourceGainData),
                Entity.Add(new OwnerData()),
                Entity.Add(CapturableData.Default)
            };
        }
    }
}