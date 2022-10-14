using UnityEngine;

namespace _Game.Scripts.BaseUI {
    public abstract class ProgressBar : MonoBehaviour {
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }
        
        private float _currentValue;
        public float CurrentValue {
            get => _currentValue;
            set {
                _currentValue = value;
                _currentProgress = (_currentValue - MinValue) / (MaxValue - MinValue);
                UpdateProgress(_currentProgress);
            }
        }

        private float _currentProgress;
        public float CurrentProgress {
            get => _currentProgress;
            set {
                _currentProgress = value;
                // value was set
                if (MinValue != MaxValue) {
                    _currentValue = _currentProgress * (MaxValue - MinValue) + MinValue;
                }
                UpdateProgress(_currentProgress);
            }
        }

        public void Load(float minValue, float maxValue) {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        protected abstract void UpdateProgress(float progress);
    }
}