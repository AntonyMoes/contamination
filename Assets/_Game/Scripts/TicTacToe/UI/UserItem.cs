using _Game.Scripts.Lobby;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.TicTacToe.UI {
    public class UserItem : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Color _regularColor = Color.white;
        [SerializeField] private Color _currentColor = Color.green;

        private LobbyUser.Data _userData;
        public string Id => _userData.Id;

        public void SetData(LobbyUser.Data userData, bool current) {
            _userData = userData;
            _name.text = userData.Name;
            _name.color = current ? _currentColor : _regularColor;
        }
    }
}
