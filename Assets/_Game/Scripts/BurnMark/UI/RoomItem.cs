using System;
using System.Linq;
using _Game.Scripts.BurnMark.Network;
using _Game.Scripts.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.BurnMark.UI {
    public class RoomItem : MonoBehaviour {
        [SerializeField] private Slot[] _slots;
        [SerializeField] private Button _joinButton;
        [SerializeField] private TMP_InputField _passwordInput;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _startGameButton;

        private Room<RoomSettings>.Data _roomData;
        public string Id => _roomData.Id;

        public void SetData(Room<RoomSettings>.Data roomData, LobbyUser.Data currentUser,
            Action<string, string> onJoinRoom, Action onLeaveRoom, Action onStartGame) {
            _roomData = roomData;

            for (var i = 0; i < RoomSettings.MaxUsers; i++) {
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

                var canStartGame = containsCurrent && _roomData.Settings.CanStartGame(currentUser);
                _startGameButton.gameObject.SetActive(canStartGame);
                _startGameButton.onClick.SetOnlyListener(onStartGame);
            }
        }

        [Serializable]
        private struct Slot {
            public UserItem _userItem;
        }
    }
}