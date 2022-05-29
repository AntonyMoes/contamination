using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class Tile : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _label;

        public Button.ButtonClickedEvent OnClick => _button.onClick;

        public void SetMark(TicTacToeState.EMark mark) {
            _label.text = mark switch {
                TicTacToeState.EMark.None => "-",
                TicTacToeState.EMark.X => "X",
                TicTacToeState.EMark.O => "O",
                _ => throw new ArgumentOutOfRangeException(nameof(mark), mark, null)
            };
        }
    }
}
