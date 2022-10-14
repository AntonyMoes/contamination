using System;
using GeneralUtils;
using UnityEngine;

namespace _Game.Scripts.BaseUI {
    public class UIElement : MonoBehaviour {
        public EState State => _state.Value;
        private readonly UpdatedValue<EState> _state = new UpdatedValue<EState>(EState.Hided);

        protected virtual bool ClearOnHide => true;

        public void Show(Action onDone = null) {
            if (State == EState.Shown) {
                onDone?.Invoke();
                return;
            }

            if (State == EState.Showing) {
                _state.WaitFor(EState.Shown, onDone);
                return;
            }
            
            // TODO what to do if Hiding?

            _state.Value = EState.Showing;
            gameObject.SetActive(true);

            PerformShow(() => {
                _state.Value = EState.Shown;
                onDone?.Invoke();
            });
        }

        protected virtual void PerformShow(Action onDone = null) {
            onDone?.Invoke();
        }

        public void Hide(Action onDone = null) {
            if (State == EState.Hided) {
                onDone?.Invoke();
                return;
            }

            if (State == EState.Hiding) {
                _state.WaitFor(EState.Hided, onDone);
                return;
            }

            // TODO what to do if Showing?

            _state.Value = EState.Hiding;

            PerformHide(() => {
                gameObject.SetActive(false);
                _state.Value = EState.Hided;

                if (ClearOnHide) {
                    Clear();
                }

                onDone?.Invoke();
            });
        }

        protected virtual void PerformHide(Action onDone = null) {
            onDone?.Invoke();
        }

        public virtual void Clear() { }

        public enum EState {
            Showing,
            Shown,
            Hiding,
            Hided
        }
    }
}