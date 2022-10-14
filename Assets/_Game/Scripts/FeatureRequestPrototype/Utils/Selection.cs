using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.GameObjects;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using _Game.Scripts.FeatureRequestPrototype.UI;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Utils {
    public static class Selection {
        private class EmployeeSelectorGroup : IDisposable {
            private readonly Action<EmployeeSelector[]> _onSelect;
            private readonly List<EmployeeSelector> _selectors;

            public EmployeeSelectorGroup(EmployeeSelector.SelectionType type, Action<EmployeeSelector[]> onSelect, params EmployeeSelector[] selectors) {
                _onSelect = onSelect;
                _selectors = selectors.ToList();

                foreach (var selector in _selectors) {
                    selector.SetType(type);
                    selector.SetActive(true);
                    selector.Button.OnClick.Subscribe(OnClick);
                    selector.Button.OnHover.Subscribe(OnHover);
                }
            }

            private void OnClick(SimpleButton _) {
                _onSelect(_selectors.ToArray());
            }

            private void OnHover(bool isHovering) {
                foreach (var selector in _selectors) {
                    selector.SetSelected(isHovering);
                }
            }

            public void Dispose() {
                foreach (var selector in _selectors) {
                    if (selector == null) {
                        return;
                    }

                    selector.SetActive(false);
                    selector.Button.OnClick.Unsubscribe(OnClick);
                    selector.Button.OnHover.Unsubscribe(OnHover);
                }

                _selectors.Clear();
            }
        }

        public class EmployeeSelectionProcess {
            private readonly List<EmployeeSelectorGroup> _groups;
            
            public EmployeeSelectionProcess(EmployeeObject[][] employeeGroups, EmployeeSelector.SelectionType type,
                Action<EmployeeObject[]> onSelected) {
                _groups = employeeGroups
                    .Select(employees => employees.Select(employee => employee.Selector).ToArray())
                    .Select(selectors => new EmployeeSelectorGroup(type, OnSelect, selectors))
                    .ToList();

                void OnSelect(EmployeeSelector[] selected) {
                    Clear();

                    var selectedEmployees = employeeGroups
                        .First(employees => employees
                            .Zip(selected, (employee, selector) => (employee, selector))
                            .All(pair => pair.employee.Selector == pair.selector));
                    Debug.Log($"Selected indices: {string.Join(",", selectedEmployees.Select(e => e.Employee.Position))}");

                    onSelected?.Invoke(selectedEmployees);
                }
            }

            private void Clear() {
                foreach (var group in _groups) {
                    group.Dispose();
                }

                _groups.Clear();
            }

            public void Abort() {
                Clear();
            }
        }

        public class SkillSelectionProcess {
            private readonly SerialProcess _process;

            public SkillSelectionProcess(Employee employee, Skill skill, EmployeeObject[] enemies, EmployeeObject[] allies,
                Action<EmployeeObject[], EmployeeObject[]> onTargetsSelected) {
                var selectedEnemies = Array.Empty<EmployeeObject>();
                var selectedAllies = Array.Empty<EmployeeObject>();
                var selectionProcess = new SerialProcess();
                EmployeeSelectionProcess currentSelectionProcess = null;

                var (enemyPositions, allyPositions) = skill.GetTargets(employee);

                if (enemyPositions.Length != 0) {
                    var enemyGroups = MapPositions(enemyPositions, enemies);
                    selectionProcess.Add(Select(enemyGroups, true, selected => selectedEnemies = selected));
                    selectionProcess.Add(new SyncProcess(() => currentSelectionProcess = null));
                }

                if (allyPositions.Length != 0) {
                    var allyGroups = MapPositions(allyPositions, allies);
                    selectionProcess.Add(Select(allyGroups, false, selected => selectedAllies = selected));
                    selectionProcess.Add(new SyncProcess(() => currentSelectionProcess = null));
                }

                _process = selectionProcess;
                _process.Run(() => {
                    onTargetsSelected(selectedEnemies, selectedAllies);
                }, () => {
                    currentSelectionProcess?.Abort();
                    currentSelectionProcess = null;
                });

                static EmployeeObject[][] MapPositions(int[][] positions, EmployeeObject[] employees) {
                    return positions
                        .Select(group => group
                            .Select(employees.WithPosition)
                            .ToArray())
                        .ToArray();
                }

                AsyncProcess Select(EmployeeObject[][] employeeGroups, bool areEnemies, Action<EmployeeObject[]> setSelected) {
                    var process = new AsyncProcess(onDone => {
                        var type = areEnemies ? EmployeeSelector.SelectionType.Enemy : EmployeeSelector.SelectionType.Ally;

                        currentSelectionProcess = new EmployeeSelectionProcess(employeeGroups, type, selected => {
                            setSelected(selected);
                            onDone?.Invoke();
                        });
                    });

                    return process;
                }
            }

            public void Abort() {
                _process.TryAbort();
            }
        }
    }
}