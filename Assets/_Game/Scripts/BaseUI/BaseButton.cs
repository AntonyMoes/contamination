using System;
using UnityEngine;
using UnityEngine.UI;
using Event = GeneralUtils.Event;

namespace _Game.Scripts.BaseUI {
    public class BaseButton : MonoBehaviour {
        [SerializeField] private Button _button;

        private readonly Action _onClick;
        public readonly Event OnClick;

        public bool Enabled {
            get => _button.enabled;
            set => _button.enabled = value;
        }

        public BaseButton() {
            OnClick = new Event(out _onClick);
        }

        private void Awake() {
            _button.onClick.AddListener(OnClickListener);
        }

        private void OnClickListener() {
            PerformOnClick();
            _onClick();
        }

        protected virtual void PerformOnClick() { }
    }
}