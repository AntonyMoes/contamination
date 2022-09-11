using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.BaseUI;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class EmployeeSelectorGroup : IDisposable {
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

        private void OnHover(SimpleButton _, bool isHovering) {
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
}