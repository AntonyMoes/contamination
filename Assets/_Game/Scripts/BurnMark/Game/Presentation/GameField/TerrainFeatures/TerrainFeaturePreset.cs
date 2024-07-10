using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.TerrainFeatures {
    public class TerrainFeaturePreset : MonoBehaviour {
        public virtual void Init(Vector3 tileCenter, IReadOnlyComponent<TerrainData> terrainComponent, GameDataReadAPI readAPI) {
            gameObject.SetActive(true);
            transform.position = tileCenter;
        }

        public virtual void Clear() {
            gameObject.SetActive(false);
        }
    }
}