using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class BuildPanel : MonoBehaviour {
        [SerializeField] private Transform _buildItemsParent;
        [SerializeField] private BuildItem _buildItemPrefab;
        [SerializeField] private Transform _buildQueueItemsParent;
        [SerializeField] private BuildQueueItem _buildQueueItemPrefab;

        private readonly System.Action<int, int> _onBuild;
        public readonly Event<int, int> OnBuild;

        private readonly System.Action<int, int> _onCancelBuild;
        public readonly Event<int, int> OnCancelBuild;

        private readonly List<BuildItem> _buildItems = new List<BuildItem>();
        private readonly List<BuildQueueItem> _buildQueueItems = new List<BuildQueueItem>();

        private IReadOnlyComponent<UnitBuilderData> _builder;
        private IReadOnlyComponent<ResourceData> _resources;

        public BuildPanel() {
            OnBuild = new Event<int, int>(out _onBuild);
            OnCancelBuild = new Event<int, int>(out _onCancelBuild);
        }

        public void Initialize(IReadOnlyComponent<ResourceData> resources, IReadOnlyComponent<UnitBuilderData> builder) {
            _resources = resources;
            _resources.OnDataUpdate.Subscribe(OnResourceUpdate);
            _builder = builder;
            _builder.OnDataUpdate.Subscribe(OnBuilderUpdate);
            FillData(builder);
        }

        private void OnBuilderUpdate(UnitBuilderData? _, IReadOnlyComponent<UnitBuilderData> data) {
            ClearData();
            FillData(data);
        }

        private void OnResourceUpdate(ResourceData? _, IReadOnlyComponent<ResourceData> data) {
            foreach (var item in _buildItems) {
                item.SetState(_resources.Data);
            }
        }

        private void FillData(IReadOnlyComponent<UnitBuilderData> component) {
            var data = component.Data;

            for (var index = 0; index < data.BuildableUnits.Length; index++) {
                var idx = index;
                var unit = data.BuildableUnits[idx];
                var item = Instantiate(_buildItemPrefab, _buildItemsParent);
                item.Initialize(unit.Icon, unit.Name, unit.Cost, unit.WorkToBuild);
                item.SetState(_resources.Data);
                item.OnBuild.Subscribe(() => _onBuild(component.ReadOnlyEntity.Id, idx));
                _buildItems.Add(item);
            }

            if (data.Current == null) {
                return;
            }

            var queue = data.Queue.Prepend(data.Current).ToArray();
            for (var i = 0; i < queue.Length; i++) {
                var idx = i;
                var unit = queue[idx];
                var item = Instantiate(_buildQueueItemPrefab, _buildQueueItemsParent);
                item.Initialize(unit.Icon, unit.Name, unit.WorkToBuild);
                item.SetState(idx == 0, data.WorkLeft);
                item.OnCancel.Subscribe(() => _onCancelBuild(component.ReadOnlyEntity.Id, idx - 1));
                _buildQueueItems.Add(item);
            }
        }

        private void ClearData() {
            foreach (var item in _buildItems) {
                Destroy(item.gameObject);
            }
            _buildItems.Clear();

            foreach (var item in _buildQueueItems) {
                Destroy(item.gameObject);
            }
            _buildQueueItems.Clear();
        }

        public void Clear() {
            ClearData();
            _resources?.OnDataUpdate.Unsubscribe(OnResourceUpdate);
            _builder?.OnDataUpdate.Unsubscribe(OnBuilderUpdate);
        }
    }
}