using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;
using _Game.Scripts.FeatureRequestPrototype.GameObjects;
using _Game.Scripts.FeatureRequestPrototype.Logic;
using _Game.Scripts.FeatureRequestPrototype.Logic.Skills;
using _Game.Scripts.FeatureRequestPrototype.UI;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Utils {
    public static class Selection {
        private class EmployeeSelectorGroup : IDisposable {
            private readonly Action<EmployeeSelector[]> _onSelect;
            private readonly List<EmployeeSelector> _selectors;

            public EmployeeSelectorGroup(EmployeeSelector.ESelectionType type, Action<EmployeeSelector[]> onSelect, params EmployeeSelector[] selectors) {
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
            
            public EmployeeSelectionProcess(EmployeeObject[][] employeeGroups, EmployeeSelector.ESelectionType type,
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

            public SkillSelectionProcess(Employee employee, ISkill skill, EmployeeObject[] enemies, EmployeeObject[] allies,
                Action<Employee[], Employee[]> onTargetsSelected) {
                var selectedEnemies = Array.Empty<Employee>();
                var selectedAllies = Array.Empty<Employee>();
                var selectionProcess = new SerialProcess();
                EmployeeSelectionProcess currentSelectionProcess = null;

                var targets = skill.GetTargets(employee);

                selectionProcess.Add(Select(targets, allies, ESkillTarget.Ally,
                    selected => selectedAllies = selected, process => currentSelectionProcess = process));

                selectionProcess.Add(Select(targets, allies, ESkillTarget.MovePosition,
                    selected => selectedAllies = selected, process => currentSelectionProcess = process));

                selectionProcess.Add(Select(targets, enemies, ESkillTarget.Enemy,
                    selected => selectedEnemies = selected, process => currentSelectionProcess = process));

                _process = selectionProcess;
                _process.Run(() => {
                    onTargetsSelected(selectedEnemies, selectedAllies);
                }, () => {
                    currentSelectionProcess?.Abort();
                    currentSelectionProcess = null;
                });

                static EmployeeObject[][] MapPositions(int[][] positions, EmployeeObject[] employees) {
                    // TODO: no empty slots handling
                    return positions
                        .Select(group => group
                            .Select(employees.WithPosition)
                            .ToArray())
                        .ToArray();
                }

                static Process Select(IDictionary<ESkillTarget, int[][]> targets, EmployeeObject[] employees, ESkillTarget target, Action<Employee[]> setSelected, Action<EmployeeSelectionProcess> setSelectionProcess) {
                    if (!targets.TryGetValue(target, out var positions) || positions.Length == 0) {
                        return new DummyProcess();
                    }

                    // TODO: no empty slots handling
                    var employeeGroups = MapPositions(positions, employees);
                    var selectionType = target switch {
                        ESkillTarget.Enemy => EmployeeSelector.ESelectionType.Enemy,
                        ESkillTarget.Ally => EmployeeSelector.ESelectionType.Ally,
                        ESkillTarget.MovePosition => EmployeeSelector.ESelectionType.MovePosition,
                        _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
                    };

                    var selectionProcess = new AsyncProcess(onDone => {
                        var currentSelectionProcess = new EmployeeSelectionProcess(employeeGroups, selectionType, selected => {
                            setSelected(selected.Select(e => e.Employee).ToArray());
                            onDone?.Invoke();
                        });
                        setSelectionProcess(currentSelectionProcess);
                    });
                    var cleanupProcess = new SyncProcess(() => setSelectionProcess(null));

                    var process = new SerialProcess();
                    process.Add(selectionProcess);
                    process.Add(cleanupProcess);
                    return process;
                }
            }

            public void Abort() {
                _process.TryAbort();
            }
        }
    }
}