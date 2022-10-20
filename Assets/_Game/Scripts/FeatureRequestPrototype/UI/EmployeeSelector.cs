using System;
using System.Linq;
using _Game.Scripts.BaseUI;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.FeatureRequestPrototype.UI {
    public class EmployeeSelector : MonoBehaviour {
        [Header("Components and settings")]
        [SerializeField] private Image[] _selectorColoredImages;
        [SerializeField] private TypeColor[] _colors;
        [SerializeField] private SimpleButton _selectorButton;

        public SimpleButton Button => _selectorButton;

        [Header("Selection and animation")]
        [SerializeField] private GameObject _baseGroup;
        [SerializeField] private GameObject _selectionGroup;

        public void SetActive(bool active) {
            _baseGroup.SetActive(active);
            SetSelected(false);
        }

        public void SetType(ESelectionType type) {
            var color = _colors.First(c => c.type == type).color;
            foreach (var image in _selectorColoredImages) {
                image.color = color;
            }
        }

        public void SetSelected(bool selected) {
            if (!selected) {
                _selectionGroup.SetActive(false);
                return;
            }

            _selectionGroup.SetActive(true);
            // TODO animation
        }

        public enum ESelectionType {
            UnitToUse,
            Current,
            Ally,
            Enemy,
            MovePosition
        }
        
        [Serializable]
        private struct TypeColor {
            public ESelectionType type;
            public Color color;
        }
    }
}