using System.IO;
using _Game.Scripts.FeatureRequestPrototype.Utils;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Data {
    public class ArtStorage : SingletonBehaviour<ArtStorage> {
        private const string Location = "FeatureRequestPrototype";
        
        public Sprite GetSprite(string spriteName) {
            return Resources.Load<Sprite>(Path.Combine(Location, spriteName));
        }
    }
}