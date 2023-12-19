using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    [CreateAssetMenu(menuName = Configs.MenuItem + nameof(FactionConfig), fileName = nameof(FactionConfig))]
    public class FactionConfig : Config {
        [SerializeField] private string _id;
        public string Id => _id;

        [SerializeField] private BaseConfig _base;
        public BaseConfig Base => _base;
    }
}