using System;
using UnityEngine;

namespace _Game.Scripts.Utils {
    public class MouseDetector : MonoBehaviour {
        private readonly Action _mouseOver;
        public readonly GeneralUtils.Event MouseOver;
        private readonly Action _mouseEnter;
        public readonly GeneralUtils.Event MouseEnter;
        private readonly Action _mouseExit;
        public readonly GeneralUtils.Event MouseExit;

        public MouseDetector() {
            MouseOver = new GeneralUtils.Event(out _mouseOver);
            MouseEnter = new GeneralUtils.Event(out _mouseEnter);
            MouseExit = new GeneralUtils.Event(out _mouseExit);
        }

        private void OnMouseOver() {
            _mouseOver();
        }

        private void OnMouseEnter() {
            _mouseEnter();
        }

        private void OnMouseExit() {
            _mouseExit();
        }
    }
}