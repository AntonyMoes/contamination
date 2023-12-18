using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class EntityPanel : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _entityName;

        [Header("Info items")]
        [SerializeField] private Transform _infoItemsParent;
        [SerializeField] private EntityInfoItem _infoItemPrefab;
        [SerializeField] private Sprite _healthIcon;
        [SerializeField] private Sprite _armorIcon;
        [SerializeField] private Sprite _moveIcon;
        [SerializeField] private Sprite _attackIcon;
        [SerializeField] private Sprite _armorPiercingIcon;
        [SerializeField] private Sprite _attackRangeIcon;

        [Header("Action items")]
        [SerializeField] private GameObject _actionItemsGroup;
        [SerializeField] private Transform _actionItemsParent;
        [SerializeField] private EntityActionItem _actionItemPrefab;

        [Header("Build")]
        [SerializeField] private BuildPanel _buildPanel;

        public Event<int, int> OnBuild => _buildPanel.OnBuild;
        public Event<int, int> OnCancelBuild => _buildPanel.OnCancelBuild;

        private readonly Dictionary<Sprite, EntityInfoItem> _infoItems = new Dictionary<Sprite, EntityInfoItem>();
        private readonly Dictionary<EntityActionConfig, EntityActionItem> _actionItems =
            new Dictionary<EntityActionConfig, EntityActionItem>();

        private GameDataReadAPI _readAPI;
        private IReadOnlyEntity _entity;
        private Action<IReadOnlyEntity, GameCommand> _onEntityCommandClicked;

        public void SetReadAPI(GameDataReadAPI readAPI) {
            _readAPI = readAPI;
        }

        public void Initialize(IReadOnlyEntity entity, Action<IReadOnlyEntity, GameCommand> onEntityCommandClicked) {
            _buildPanel.Clear();

            _entity = entity;
            _onEntityCommandClicked = onEntityCommandClicked;
            var config = entity.TryGetFieldEntityConfig()!;
            _entityName.text = config.Name;

            SetInfoItems(entity);
            SetActions(entity);
            SetBuild(entity);
        }

        private void SetInfoItems(IReadOnlyEntity entity) {
            foreach (var item in _infoItems.Values) {
                item.gameObject.SetActive(false);
            }

            var healthData = entity.GetReadOnlyComponent<HealthData>()!.Data;
            var healthItem = GetInfoItem(_healthIcon);
            var armorItem = GetInfoItem(_armorIcon);
            healthItem.SetDivisive(healthData.Health, healthData.MaxHealth);
            armorItem.Set(healthData.Armor);

            if (entity.GetReadOnlyComponent<MoveData>()?.Data is { } moveData) {
                var moveItem = GetInfoItem(_moveIcon);
                moveItem.Set(moveData.Distance);
            }

            if (entity.GetReadOnlyComponent<AttackData>()?.Data is { } attackData) {
                var attackItem = GetInfoItem(_attackIcon);
                attackItem.SetMultiplicative(attackData.Damage, attackData.Attacks);
                var armorPiercingItem = GetInfoItem(_armorPiercingIcon);
                armorPiercingItem.Set(attackData.ArmorPiercing);
                var attackRangeItem = GetInfoItem(_attackRangeIcon);
                attackRangeItem.Set(attackData.Range);
            }
        }

        private void SetActions(IReadOnlyEntity entity) {
            foreach (var item in _actionItems.Values) {
                item.gameObject.SetActive(false);
            }

            var config = entity.TryGetFieldEntityConfig()!;
            _actionItemsGroup.SetActive(config.Actions.Length > 0);
            foreach (var action in config.Actions) {
                var item = GetActionItem(action);
                item.gameObject.SetActive(true);
            }
        }

        private void SetBuild(IReadOnlyEntity entity) {
            var builder = entity.GetReadOnlyComponent<UnitBuilderData>();
            var resources = entity.GetInReadOnlyOwner<ResourceData>(_readAPI);
            if (builder == null || resources == null) {
                _buildPanel.gameObject.SetActive(false);
                return;
            }

            _buildPanel.gameObject.SetActive(true);
            _buildPanel.Initialize(resources, builder);
        }

        private void OnActionClick(EntityActionItem item) {
            _onEntityCommandClicked(_entity, item.GetCommand(_entity));
        }

        private EntityInfoItem GetInfoItem(Sprite icon) {
            var item = _infoItems.GetValue(icon, () => {
                var item = Instantiate(_infoItemPrefab, _infoItemsParent);
                item.Initialize(icon);
                return item;
            });
            item.gameObject.SetActive(true);
            return item;
        }

        private EntityActionItem GetActionItem(EntityActionConfig config) {
            var item = _actionItems.GetValue(config, () => {
                var item = Instantiate(_actionItemPrefab, _actionItemsParent);
                item.Initialize(config);
                item.OnClick.Subscribe(() => OnActionClick(item));
                return item;
            });
            item.gameObject.SetActive(true);
            return item;
        }

        public void Clear() {
            _buildPanel.Clear();

            foreach (var item in _infoItems.Values) {
                Destroy(item.gameObject);
            }
            _infoItems.Clear();

            foreach (var item in _actionItems.Values) {
                Destroy(item.gameObject);
            }
            _actionItems.Clear();
        }
    }
}