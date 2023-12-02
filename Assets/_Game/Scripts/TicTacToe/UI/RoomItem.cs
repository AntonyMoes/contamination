using System;
using System.Linq;
using _Game.Scripts.Lobby;
using _Game.Scripts.TicTacToe.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.TicTacToe.UI {
    public class RoomItem : MonoBehaviour {
        [SerializeField] private Slot[] _slots;
        [SerializeField] private Button _joinButton;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _switchMarksButton;
        [SerializeField] private Button _startGameButton;

        private Room<TicTacToeRoomSettings>.Data _roomData;
        public string Id => _roomData.Id;

        public void SetData(Room<TicTacToeRoomSettings>.Data roomData, LobbyUser.Data currentUser,
            Action<string, string> onJoinRoom, Action onLeaveRoom, Action<string> onSwitchMarks, Action onStartGame) {
            _roomData = roomData;

            for (var i = 0; i < _roomData.Settings.Marks.Length; i++) {
                _slots[i]._mark.text = _roomData.Settings.Marks[i].ToString();

                var userItem = _slots[i]._userItem;
                if (_roomData.Users.Length > i) {
                    userItem.gameObject.SetActive(true);

                    var userData = _roomData.Users[i];
                    userItem.SetData(userData, currentUser.Id == userData.Id);
                } else {
                    userItem.gameObject.SetActive(false);
                }

                var containsCurrent = _roomData.Users.Any(u => u.Id == currentUser.Id);
                var canJoin = !containsCurrent && roomData.Settings.CanJoin(currentUser);
                _joinButton.gameObject.SetActive(canJoin);
                _joinButton.onClick.SetOnlyListener(() => {
                    onJoinRoom(_roomData.Id, _passwordInput.text);
                    _passwordInput.text = string.Empty;
                });
                _passwordInput.gameObject.SetActive(canJoin && roomData.HasPassword);
                _leaveButton.gameObject.SetActive(containsCurrent);
                _leaveButton.onClick.SetOnlyListener(onLeaveRoom);

                var canSwitchMarks = containsCurrent && _roomData.Settings.CanMakeUpdate(currentUser,
                    new TicTacToeRoomSettings { UpdateType = TicTacToeRoomSettings.EUpdateType.SwitchMarks });
                _switchMarksButton.gameObject.SetActive(canSwitchMarks);
                _switchMarksButton.onClick.SetOnlyListener(() => onSwitchMarks(Id));

                var canStartGame = containsCurrent && _roomData.Settings.CanStartGame(currentUser);
                _startGameButton.gameObject.SetActive(canStartGame);
                _startGameButton.onClick.SetOnlyListener(onStartGame);
            }
        }

        [Serializable]
        private struct Slot {
            public TextMeshProUGUI _mark;
            public UserItem _userItem;
        }
    }
}