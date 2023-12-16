using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Data.Configs.EntityActions;
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

        private readonly Dictionary<Sprite, EntityInfoItem> _infoItems = new Dictionary<Sprite, EntityInfoItem>();
        private readonly Dictionary<EntityActionConfig, EntityActionItem> _actionItems =
            new Dictionary<EntityActionConfig, EntityActionItem>();

        private IReadOnlyEntity _entity;
        private Action<GameCommand> _onEntityCommandClicked;

        public void Initialize(IReadOnlyEntity entity, Action<GameCommand> onEntityCommandClicked) {
            _entity = entity;
            _onEntityCommandClicked = onEntityCommandClicked;
            var config = entity.TryGetFieldEntityConfig()!;
            _entityName.text = config.Name;

            SetInfoItems(entity);
            SetActions(entity);
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

        private void OnActionClick(EntityActionItem item) {
            _onEntityCommandClicked(item.GetCommand(_entity));
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