using System;
using _Game.Scripts.BaseUI;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class SkillItem : MonoBehaviour {
        [SerializeField] private SimpleButton _button;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private GameObject _frame;

        public Skill Skill { get; private set; }

        public void Load(Skill skill, bool canUse, Action<SkillItem, bool> onHover, Action<SkillItem> onClick) {
            Skill = skill;
            _name.text = skill.Name;

            _button.Interactable = canUse;
            _button.OnHover.Subscribe((_, hover) => onHover(this, hover));
            _button.OnClick.Subscribe(_ => onClick(this));
        }

        public void SetCurrent(bool current) {
            _frame.SetActive(current);
        }

        private void OnDestroy() {
            _button.OnHover.ClearSubscribers();
            _button.OnClick.ClearSubscribers();
        }
    }
}