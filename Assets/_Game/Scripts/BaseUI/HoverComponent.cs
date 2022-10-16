using GeneralUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.Scripts.BaseUI {
    public class HoverComponent : MonoBehaviour {
        public static HoverComponent Create(Image hoverZone, UpdatedValue<bool> onHover) {
            var hover = hoverZone.gameObject.AddComponent<HoverComponent>();
            
            var eventTrigger = hoverZone.TryGetComponent<EventTrigger>(out var trigger)
                ? trigger
                : hoverZone.gameObject.AddComponent<EventTrigger>();

            var enter = new EventTrigger.Entry {
                eventID = EventTriggerType.PointerEnter,
                callback = new EventTrigger.TriggerEvent()
            };
            enter.callback.AddListener(_ => onHover.Value = true);
            eventTrigger.triggers.Add(enter);

            var exit = new EventTrigger.Entry {
                eventID = EventTriggerType.PointerExit,
                callback = new EventTrigger.TriggerEvent()
            };
            exit.callback.AddListener(_ => onHover.Value = false);
            eventTrigger.triggers.Add(exit);

            return hover;
        }
    }
}