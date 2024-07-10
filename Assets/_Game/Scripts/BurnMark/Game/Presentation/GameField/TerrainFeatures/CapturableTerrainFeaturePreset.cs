using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using UnityEngine;
using UnityEngine.UI;
using TerrainData = _Game.Scripts.BurnMark.Game.Data.Components.TerrainData;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameField.TerrainFeatures {
    public class CapturableTerrainFeaturePreset : TerrainFeaturePreset {
        [SerializeField]private Image _ownerFrame;
        [SerializeField] private Color _defaultColor;

        private bool _clear = true;
        private IReadOnlyComponent<TerrainData> _terrainComponent;
        private GameDataReadAPI _readAPI;

        public override void Init(Vector3 tileCenter, IReadOnlyComponent<TerrainData> terrainComponent, GameDataReadAPI readAPI) {
            base.Init(tileCenter, terrainComponent, readAPI);
            _ownerFrame.gameObject.SetActive(true);

            _readAPI = readAPI;
            _terrainComponent = terrainComponent;
            _terrainComponent.ReadOnlyEntity.GetReadOnlyComponent<OwnerData>()!.OnDataUpdate.Subscribe(OnOwnerUpdate);
            _clear = false;
        }

        public override void Clear() {
            if (_clear) {
                return;
            }

            base.Clear();
            _ownerFrame.gameObject.SetActive(false);

            _terrainComponent.ReadOnlyEntity.GetReadOnlyComponent<OwnerData>()!.OnDataUpdate.Unsubscribe(OnOwnerUpdate);
            _clear = true;
        }

        private void OnOwnerUpdate(OwnerData? _, IReadOnlyComponent<OwnerData> ownerComponent) {
            var color = Game.Entities.Utils.GetInReadOnlyOwner<PlayerData>(ownerComponent.Data.Owner, _readAPI)?.Data.Color ?? _defaultColor;
            _ownerFrame.color = color;
        }
    }
}