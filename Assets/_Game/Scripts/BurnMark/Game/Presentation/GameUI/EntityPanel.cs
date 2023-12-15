using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.ModelV4.ECS;
using GeneralUtils;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
    public class EntityPanel : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _entityName;
        [SerializeField] private Transform _infoItemsParent;
        [SerializeField] private EntityInfoItem _infoItemPrefab;
        [SerializeField] private Sprite _healthIcon;
        [SerializeField] private Sprite _armorIcon;
        [SerializeField] private Sprite _attackIcon;
        [SerializeField] private Sprite _armorPiercingIcon;
        [SerializeField] private Sprite _attackRangeIcon;

        private readonly Dictionary<Sprite, EntityInfoItem> _items = new Dictionary<Sprite, EntityInfoItem>();

        public void Initialize(IReadOnlyEntity entity) {
            var config = entity.TryGetFieldEntityConfig()!;
            _entityName.text = config.Name;

            var healthItem = GetItem(_healthIcon);
            var armorItem = GetItem(_armorIcon);
            var healthData = entity.GetReadOnlyComponent<HealthData>()!.Data;
            healthItem.InitializeDivisive(healthData.Health, healthData.MaxHealth);
            armorItem.Initialize(healthData.Armor);

            var attackItem = GetItem(_attackIcon);
            var armorPiercingItem = GetItem(_armorPiercingIcon);
            var attackRangeItem = GetItem(_attackRangeIcon);
            if (entity.GetReadOnlyComponent<AttackData>()?.Data is { } attackData) {
                attackItem.gameObject.SetActive(true);
                attackItem.InitializeMultiplicative(attackData.Damage, attackData.Attacks);
                armorPiercingItem.gameObject.SetActive(true);
                armorPiercingItem.Initialize(attackData.ArmorPiercing);
                attackRangeItem.gameObject.SetActive(true);
                attackRangeItem.Initialize(attackData.Range);
            } else {
                attackItem.gameObject.SetActive(false);
                armorPiercingItem.gameObject.SetActive(false);
                attackRangeItem.gameObject.SetActive(false);
            }
        }

        private EntityInfoItem GetItem(Sprite icon) {
            return _items.GetValue(icon, () => {
                var item = Instantiate(_infoItemPrefab, _infoItemsParent);
                item.SetIcon(icon);
                return item;
            });
        }
    }
}