using System;
using GeneralUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.Scripts.BaseUI {
    public class SimpleButton : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private Image _hoverZone;

        private readonly Action<SimpleButton> _onClick;
        public readonly Event<SimpleButton> OnClick;

        private readonly UpdatedValue<bool> _onHover = new UpdatedValue<bool>(false);
        public IUpdatedValue<bool> OnHover => _onHover;

        public bool Interactable {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public SimpleButton() {
            OnClick = new Event<SimpleButton>(out _onClick);
        }

        private void Awake() {
            _button.onClick.AddListener(() => _onClick(this));

            if (_hoverZone == null) {
                return;
            }

            var eventTrigger = _hoverZone.TryGetComponent<EventTrigger>(out var trigger)
                ? trigger
                : _hoverZone.gameObject.AddComponent<EventTrigger>();

            var enter = new EventTrigger.Entry {
                eventID = EventTriggerType.PointerEnter,
                callback = new EventTrigger.TriggerEvent()
            };
            enter.callback.AddListener(_ => _onHover.Value = true);
            eventTrigger.triggers.Add(enter);

            var exit = new EventTrigger.Entry {
                eventID = EventTriggerType.PointerExit,
                callback = new EventTrigger.TriggerEvent()
            };
            exit.callback.AddListener(_ => _onHover.Value = false);
            eventTrigger.triggers.Add(exit);
        }
    }
}