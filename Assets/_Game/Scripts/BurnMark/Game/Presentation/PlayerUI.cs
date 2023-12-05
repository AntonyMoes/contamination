using System;
using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel.User;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class PlayerUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI _moneyCounter;
        [SerializeField] private TextMeshProUGUI _metalCounter;
        [SerializeField] private TextMeshProUGUI _turn;
        [SerializeField] private TextMeshProUGUI _currentPlayer;
        [SerializeField] private BaseButton _endTurn;
        [SerializeField] private BaseButton _testEndGame;
        [SerializeField] private GameObject _gameEndPopup;
        [SerializeField] private TextMeshProUGUI _gameEndPopupText;
        [SerializeField] private BaseButton _exitGameButton;

        private int _player;
        private GameDataReadAPI _readAPI;
        private GameDataEventsAPI _eventsAPI;
        private Action _onPlayerClosedGame;

        public void Initialize(int player, GameDataEventsAPI eventsAPI, Action endTurn, Action testEndGame, Action onPlayerClosedGame) {
            _player = player;
            _eventsAPI = eventsAPI;
            _eventsAPI.GetComponentUpdateEvent<ResourceData>().Subscribe(OnResourceUpdate);
            _eventsAPI.OnTurnChanged.Subscribe(OnTurnChanged);
            _endTurn.Enabled = false;
            _endTurn.OnClick.Subscribe(endTurn);
            _testEndGame.Enabled = false;
            _testEndGame.OnClick.Subscribe(testEndGame);
            _onPlayerClosedGame = onPlayerClosedGame;
            _exitGameButton.OnClick.Subscribe(OnPlayerClosedGame);
        }

        public void SetReadAPI(GameDataReadAPI readAPI) {
            _readAPI = readAPI;
        }

        public void OnGameEnded(bool victory) {
            _eventsAPI.GetComponentUpdateEvent<ResourceData>().Unsubscribe(OnResourceUpdate);
            _eventsAPI.OnTurnChanged.Unsubscribe(OnTurnChanged);

            _gameEndPopupText.text = victory ? "VICTORY!" : "Defeat :c";
            _gameEndPopup.gameObject.SetActive(true);
        }

        private void OnPlayerClosedGame() {
            _endTurn.OnClick.ClearSubscribers();
            _testEndGame.OnClick.ClearSubscribers();
            _exitGameButton.OnClick.Unsubscribe(OnPlayerClosedGame);

            _gameEndPopup.gameObject.SetActive(false);

            _onPlayerClosedGame?.Invoke();
        }

        private void OnTurnChanged(IReadOnlyUser oldUser, IReadOnlyUser newUser) {
            _currentPlayer.text = newUser.Name;
            _turn.text = _readAPI.CurrentTurn.ToString();
            _endTurn.Enabled = newUser.Id == _player;
            _testEndGame.Enabled = newUser.Id == _player;
        }

        private void OnResourceUpdate(ResourceData oldData, IReadOnlyComponent<ResourceData> newData) {
            var owner = newData.ReadOnlyEntity.GetOwnerId();
            Debug.LogWarning($"UPDATE for {owner} in {_player}");
            if (owner != _player) {
                return;
            }

            _moneyCounter.text = newData.Data.Money.ToString();
            _metalCounter.text = newData.Data.Metal.ToString();
        }
    }
}