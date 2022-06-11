using System;
using _Game.Scripts.ModelV4.NetTicTacToeExample.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.ModelV4.NetTicTacToeExample {
    public class Tile : MonoBehaviour {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _label;

        public Button.ButtonClickedEvent OnClick => _button.onClick;

        public void SetMark(MarkData.EMark mark) {
            _label.text = mark switch {
                MarkData.EMark.None => "-",
                MarkData.EMark.X => "X",
                MarkData.EMark.O => "O",
                _ => throw new ArgumentOutOfRangeException(nameof(mark), mark, null)
            };
        }
    }
}
