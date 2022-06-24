using System;
using System.Linq;
using _Game.Scripts.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.Scripts.TicTacToe {
    public class RoomItem : MonoBehaviour {
        [SerializeField] private Slot[] _slots;
        [SerializeField] private Button _joinButton;
        [FormerlySerializedAs("_passwordField")] [SerializeField] private TMP_InputField _passwordInput; 
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _switchMarksButton;
        [SerializeField] private Button _startGameButton;

        private Room<TicTacToeRoomSettings>.Data _roomData;
        public string Id => _roomData.Id;

        public void SetData(Room<TicTacToeRoomSettings>.Data roomData, string currentUserId, Action<string, string> onJoinRoom, Action onLeaveRoom) {
            _roomData = roomData;

            for (var i = 0; i < _roomData.Settings.Marks.Length; i++) {
                _slots[i]._mark.text = _roomData.Settings.Marks[i].ToString();

                var userItem = _slots[i]._userItem;
                if (_roomData.Users.Length > i) {
                    userItem.gameObject.SetActive(true);

                    var userData = _roomData.Users[i];
                    userItem.SetData(userData, currentUserId == userData.Id);
                } else {
                    userItem.gameObject.SetActive(false);
                }

                var containsCurrent = _roomData.Users.Any(u => u.Id == currentUserId);
                _joinButton.gameObject.SetActive(!containsCurrent);
                _joinButton.onClick.RemoveAllListeners();
                _joinButton.onClick.AddListener(() => onJoinRoom(_roomData.Id, _passwordInput.text));
                _passwordInput.gameObject.SetActive(!containsCurrent && _roomData.HasPassword);
                _leaveButton.gameObject.SetActive(containsCurrent);
                _leaveButton.onClick.RemoveAllListeners();
                _leaveButton.onClick.AddListener(() => onLeaveRoom());

                var canSwitchMarks = containsCurrent && _roomData.Settings.CanMakeUpdate(currentUserId,
                    new TicTacToeRoomSettings {UpdateType = TicTacToeRoomSettings.EUpdateType.SwitchMarks});
                _switchMarksButton.gameObject.SetActive(canSwitchMarks);
                // TODO

                var canStartGame = containsCurrent && _roomData.Settings.CanStartGame(currentUserId);
                _startGameButton.gameObject.SetActive(canStartGame);
                // TODO
            }
        }

        [Serializable]
        private struct Slot {
            public TextMeshProUGUI _mark;
            public UserItem _userItem;
        }
    }
}
