using System;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic.Skills;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class SkillItem : MonoBehaviour {
        [SerializeField] private SimpleButton _button;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private GameObject _frame;

        public ISkill Skill { get; private set; }

        public void Load(ISkill skill, bool canUse, Action<SkillItem, bool> onHover, Action<SkillItem> onClick) {
            Skill = skill;
            _name.text = skill.Name;

            _button.Interactable = canUse;
            _button.OnHover.Subscribe(hover => onHover(this, hover));
            _button.OnClick.Subscribe(_ => onClick(this));
        }

        public void SetCurrent(bool current) {
            _frame.SetActive(current);
        }

        private void OnDestroy() {
            _button.OnHover.Clear();
            _button.OnClick.ClearSubscribers();
        }
    }
}