using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.FeatureRequestPrototype.UI;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.FeatureRequestPrototype.Utils {
    public static class Selection {
        public static Action SelectEmployees(Employee[][] employeeGroups, Action<Employee[]> onSelected) {
            var groups = Array.Empty<EmployeeSelectorGroup>();
            groups = employeeGroups
                .Select(employees => employees.Select(employee => employee.Selector).ToArray())
                .Select(selectors => new EmployeeSelectorGroup(EmployeeSelector.SelectionType.UnitToUse, OnSelect, selectors))
                .ToArray();

            return OnAbort;

            void OnSelect(EmployeeSelector[] selected) {
                foreach (var group in groups) {
                    group.Dispose();
                }

                var selectedEmployees = employeeGroups
                    .First(employees => employees
                        .Zip(selected, (employee, selector) => (employee, selector))
                        .All(pair => pair.employee.Selector == pair.selector));
                Debug.Log($"Selected indices: {string.Join(",", selectedEmployees.Select(e => e.Position))}");

                onSelected?.Invoke(selectedEmployees);
            }

            void OnAbort() {
                foreach (var group in groups) {
                    group.Dispose();
                }
            }
        }

        public class SkillSelectionProcess {
        private readonly SerialProcess _process;
        private readonly List<Action> _aborters = new List<Action>();

        public SkillSelectionProcess(Employee employee, Skill skill, Employee[] enemies, Employee[] allies,
            /*Func<Employee[][], Action<Employee[]>, Action> employeeSelector, */Action<Employee[], Employee[]> onTargetsSelected) {
            var selectedEnemies = Array.Empty<Employee>();
            var selectedAllies = Array.Empty<Employee>();
            var selectionProcess = new SerialProcess();

            var (enemyPositions, allyPositions) = skill.GetTargets(employee);

            if (enemyPositions.Length != 0) {
                var enemyGroups = MapPositions(enemyPositions, enemies);
                selectionProcess.Add(Select(enemyGroups, selected => selectedEnemies = selected));
            }

            if (allyPositions.Length != 0) {
                var allyGroups = MapPositions(allyPositions, allies);
                selectionProcess.Add(Select(allyGroups, selected => selectedAllies = selected));
            }

            _process = selectionProcess;
            _process.Run(() => {
                onTargetsSelected(/*employee, skill, */selectedEnemies, selectedAllies);
            });

            static Employee[][] MapPositions(int[][] positions, Employee[] employees) {
                return positions
                    .Select(group => group
                        .Select(employees.WithPosition)
                        .ToArray())
                    .ToArray();
            }

            AsyncProcess Select(Employee[][] employeeGroups, Action<Employee[]> setSelected) {
                var process = new AsyncProcess(onDone => {
                    var aborter = SelectEmployees(employeeGroups, selected => {
                        setSelected(selected);
                        onDone?.Invoke();
                    });
                    _aborters.Add(aborter);
                });

                return process;
            }
        }

        public void Abort() {
            _process.TryAbort();
            _aborters.ForEach(aborter => aborter());
            _aborters.Clear();
        }
    }
    }
}