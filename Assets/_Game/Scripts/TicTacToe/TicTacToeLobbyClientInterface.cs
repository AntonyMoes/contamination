using System;
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
        private readonly List<UserItem> _users = new List<UserItem>();
        private readonly List<RoomItem> _rooms = new List<RoomItem>();
        private Action _gameStarter;

        private void Awake() {
            _connectButton.onClick.AddListener(TryConnect);
            _disconnectButton.onClick.AddListener(TryDisconnect);
            _roomCreateButton.onClick.AddListener(TryCreateRoom);
            gameObject.SetActive(false);
        }

        public void StartLobby(LobbyClient<TicTacToeRoomSettings> lobbyClient, Action gameStarter) {
            gameObject.SetActive(true);

            _lobbyClient = lobbyClient;
            _lobbyClient.OnLobbyUpdate.Subscribe(OnLobbyUpdate);
            _gameStarter = gameStarter;
            _lobbyClient.OnServerGameStart.Subscribe(_gameStarter);
            SetCurrentState();
            ReloadContent();
        }

        public void StopLobby() {
            gameObject.SetActive(false);
            EnableOverlay(false);

            _lobbyClient.Stop();
            _lobbyClient.OnLobbyUpdate.Unsubscribe(OnLobbyUpdate);
            _lobbyClient.OnServerGameStart.Unsubscribe(_gameStarter);
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
            _roomPasswordInput.text = string.Empty;

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

        private void TrySwitchMarks(string roomId) {
            var settings = new TicTacToeRoomSettings {
                UpdateType = TicTacToeRoomSettings.EUpdateType.SwitchMarks
            };

            EnableOverlay(true);
            _lobbyClient.TryUpdateRoom(roomId, null, settings, switched => {
                EnableOverlay(false);
                if (switched) {
                    ReloadContent();
                } else {
                    Debug.LogError("Could not switch marks");
                }
            });
        }

        private void TryStartGame() {
            EnableOverlay(true);
            _lobbyClient.TryStartGame(started => {
                EnableOverlay(false);
                if (started) {
                    Debug.LogWarning("Game started!");
                    _gameStarter();
                } else {
                    Debug.LogError("Could not start game");
                }
            });
        }

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

            foreach (var userData in lobbyData.Users.Where(u => !lobbyData.Rooms.Any(r => r.Users.Any(ru => ru.Id == u.Id)))) {
                var user = Instantiate(_userPrefab, _userRoot);
                user.SetData(userData, userData.Id == _lobbyClient.Id);
                _users.Add(user);
            }

            var currenUser = _lobbyClient.LobbyData.Users.First(u => u.Id == _lobbyClient.Id);
            foreach (var roomData in lobbyData.Rooms) {
                var room = Instantiate(_roomPrefab, _roomRoot);
                room.SetData(roomData, currenUser, TryJoinRoom, TryLeaveRoom, TrySwitchMarks, TryStartGame);
                _rooms.Add(room);
            }
        }

        private void EnableOverlay(bool enabled) {
            _loadingOverlay.SetActive(enabled);
        }
    }
}
