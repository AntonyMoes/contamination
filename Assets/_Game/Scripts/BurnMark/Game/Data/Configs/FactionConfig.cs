using _Game.Scripts.BurnMark.Game.Data.Configs.Entities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Game.Scripts.BurnMark.Game.Data.Configs {
    public class FactionConfig : Config {
        [SerializeField] private string _id;
        public string Id => _id;

        [SerializeField] private BaseConfig _base;
        public BaseConfig Base => _base;

#if UNITY_EDITOR
        [MenuItem(Configs.MenuItem + nameof(FactionConfig), false)]
        public static void Create() => Configs.Create<FactionConfig>();
#endif
    }
}