using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.TicTacToe {
    public class TicTacToeLobbyClientInterface : MonoBehaviour {
        [SerializeField] private GameObject _loadingOverlay;

        [SerializeField] private GameObject _disconnectedContent;
        [SerializeField] private GameObject _connectedContent;
        [SerializeField] private GameObject _createRoomContent;

        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Button _connectButton;
        [SerializeField] private Button _disconnectButton;

        [SerializeField] private TMP_InputField _roomPasswordInput;
        [SerializeField] private Button _roomCreateButton;

        [SerializeField] private Transform _roomRoot;
        [SerializeField] private RoomItem _roomPrefab;
        [SerializeField] private Transform _userRoot;
        [SerializeField] private UserItem _userPrefab;

        private LobbyClient<TicTacToeRoomSettings> _lobbyClient;
        private List<UserItem> _users = new List<UserItem>();
        private List<RoomItem> _rooms = new List<RoomItem>();

        private void Awake() {
            _connectButton.onClick.AddListener(TryConnect);
            _disconnectButton.onClick.AddListener(TryDisconnect);
            _roomCreateButton.onClick.AddListener(TryCreateRoom);
            gameObject.SetActive(false);
        }

        public void StartLobby(LobbyClient<TicTacToeRoomSettings> lobbyClient) {
            gameObject.SetActive(true);

            _lobbyClient = lobbyClient;
            _lobbyClient.OnLobbyUpdate.Subscribe(OnLobbyUpdate);
            SetCurrentState();
            ReloadContent();
        }

        public void StopLobby() {
            gameObject.SetActive(false);
            EnableOverlay(false);

            _lobbyClient.Stop();
            _lobbyClient.OnLobbyUpdate.Unsubscribe(OnLobbyUpdate);
            _lobbyClient = null;
        }

        private void TryConnect() {
            EnableOverlay(true);
            _lobbyClient.TryJoinLobby(_nameInput.text, joined => {
                EnableOverlay(false);
                if (joined) {
                    SetCurrentState();
                    ReloadContent();
                } else {
                    Debug.LogError("Could not join lobby");
                }
            });
        }

        private void TryDisconnect() {
            EnableOverlay(true);
            _lobbyClient.TryLeaveLobby(left => {
                EnableOverlay(false);
                if (left) {
                    SetCurrentState();
                    ReloadContent();
                } else {
                    Debug.LogError("WTF");
                }
            });
        }

        private void OnLobbyUpdate() {
            ReloadContent();
        }

        private void TryCreateRoom() {
            var password = string.IsNullOrEmpty(_roomPasswordInput.text) ? null : _roomPasswordInput.text;
            EnableOverlay(true);
            _lobbyClient.TryUpdateRoom(null, password, new TicTacToeRoomSettings(), created => {
                EnableOverlay(false);
                if (created) {
                    SetCurrentState();
                    ReloadContent();
                } else {
                    Debug.LogError("Could not create room");
                }
            });
        }

        private void TryJoinRoom(string roomId, string password) {
            EnableOverlay(true);
            _lobbyClient.TryJoinRoom(roomId, password, joined => {
                EnableOverlay(false);
                if (joined) {
                    SetCurrentState();
                    ReloadContent();
                } else {
                    Debug.LogError("Could not join room");
                }
            });
        }

        private void TryLeaveRoom() {
            EnableOverlay(true);
            _lobbyClient.TryLeaveRoom(left => {
                EnableOverlay(false);
                if (left) {
                    SetCurrentState();
                    ReloadContent();
                } else {
                    Debug.LogError("WTF: maybe not in room?");
                }
            });
        }

        // private void TryMakeUpdate() {
        //     
        // }

        private void SetCurrentState() {
            var state = _lobbyClient.State;

            _disconnectedContent.SetActive(state == LobbyClient<TicTacToeRoomSettings>.EState.NotConnected);
            _connectedContent.SetActive(state != LobbyClient<TicTacToeRoomSettings>.EState.NotConnected);
            _createRoomContent.SetActive(state == LobbyClient<TicTacToeRoomSettings>.EState.Connected);
        }

        void ReloadContent() {
            foreach (var user in _users) {
                Destroy(user.gameObject);
            }

            _users.Clear();

            foreach (var room in _rooms) {
                Destroy(room.gameObject);
            }

            _rooms.Clear();

            var lobbyData = _lobbyClient.LobbyData;
            if (lobbyData == null) {
                return;
            }

            foreach (var userData in lobbyData.Users.Where(u => !lobbyData.Rooms.Any(r => r.Users.Contains(u)))) {
                var user = Instantiate(_userPrefab, _userRoot);
                user.SetData(userData, userData.Id == _lobbyClient.Id);
                _users.Add(user);
            }

            foreach (var roomData in lobbyData.Rooms) {
                var room = Instantiate(_roomPrefab, _roomRoot);
                room.SetData(roomData, _lobbyClient.Id, TryJoinRoom, TryLeaveRoom);
                _rooms.Add(room);
            }
        }

        private void EnableOverlay(bool enabled) {
            _loadingOverlay.SetActive(enabled);
        }
    }
}
