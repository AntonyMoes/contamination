using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class Tile : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _label;

        public Button.ButtonClickedEvent OnClick => _button.onClick;

        public void SetMark(MarkComponent.EMark mark) {
            _label.text = mark switch {
                MarkComponent.EMark.None => "-",
                MarkComponent.EMark.X => "X",
                MarkComponent.EMark.O => "O",
                _ => throw new ArgumentOutOfRangeException(nameof(mark), mark, null)
            };
        }
    }
}
