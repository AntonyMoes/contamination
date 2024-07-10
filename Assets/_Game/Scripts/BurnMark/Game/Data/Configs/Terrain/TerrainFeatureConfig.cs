using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs.Terrain {
    [CreateAssetMenu(menuName = Configs.TerrainMenuItem + nameof(TerrainFeatureConfig), fileName = nameof(TerrainFeatureConfig))]
    public class TerrainFeatureConfig : Config {
        [SerializeField] private string _name;
        [SerializeField] private int _additionalMovementCost;
        public int AdditionalMovementCost => _additionalMovementCost;

        public virtual IEnumerable<Func<Entity, IComponent>> GetAdditionalComponents() => Enumerable.Empty<Func<Entity, IComponent>>();

        public virtual string Tooltip => _name;
    }
}