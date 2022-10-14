using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BaseUI {
    public class ImageProgressBar : ProgressBar {
        [SerializeField] private Image _image;

        protected override void UpdateProgress(float progress) {
            _image.fillAmount = progress;
        }
    }
}