using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class SkillsPanel : UIElement {
        [SerializeField] private Transform _skillContainer;
        [SerializeField] private SkillItem _skillPrefab;
        [SerializeField] private TextMeshProUGUI _skillName;

        [SerializeField] private SkillEffectsPanel _skillEffectsPanel;

        private Employee _employee;
        private readonly List<SkillItem> _skillItems = new List<SkillItem>();
        private Skill _selectedSkill;
        private Skill _displayedSkill;
        private Action<Employee, Skill> _startTargetSelection;

        public void Load(Employee employee, Employee[] enemies, Employee[] allies, Action<Employee, Skill> startTargetSelection) {
            _employee = employee;
            _startTargetSelection = startTargetSelection;

            DisplaySkill(null);

            foreach (var skill in employee.Skills) {
                var canUse = skill.CanBeUsed(employee, enemies, allies);
                
                var skillItem = Instantiate(_skillPrefab, _skillContainer);
                skillItem.Load(skill, canUse, OnSkillHover, OnSkillClick);
                _skillItems.Add(skillItem);
            }
        }

        private void OnSkillHover(SkillItem item, bool hover) {
            DisplaySkill(hover ? item.Skill : null);
        }

        private void OnSkillClick(SkillItem item) {
            var skill = item.Skill;
            if (_selectedSkill == skill) {
                return;
            }

            SelectSkill(skill);
        }

        private void SelectSkill(Skill skill) {
            if (_selectedSkill != null) {
                _skillItems.First(i => i.Skill == _selectedSkill).SetCurrent(false);
            }

            _skillItems.First(i => i.Skill == skill).SetCurrent(true);

            _selectedSkill = skill;
            DisplaySkill(skill);

            _startTargetSelection(_employee, skill);
        }

        private void DisplaySkill([CanBeNull] Skill skill) {
            _displayedSkill = skill ?? _selectedSkill;
            if (_displayedSkill == null) {
                _skillName.text = "";
                _skillEffectsPanel.Hide();
                return;
            }
            _skillName.text = _displayedSkill.Name;
            _skillEffectsPanel.Load(_displayedSkill);
            _skillEffectsPanel.Show();
        }

        protected override void PerformShow(Action onDone = null) {
            base.PerformShow(onDone);
        }

        protected override void PerformHide(Action onDone = null) {
            var hideProcess = new ParallelProcess();
            hideProcess.Add(new AsyncProcess(base.PerformHide));
            hideProcess.Add(new AsyncProcess(_skillEffectsPanel.Hide));
            hideProcess.Run(onDone);
        }

        public override void Clear() {
            foreach (var skillItem in _skillItems) {
                Destroy(skillItem.gameObject);
            }

            _skillItems.Clear();

            _selectedSkill = null;
            _displayedSkill = null;

            base.Clear();
        }
    }
}