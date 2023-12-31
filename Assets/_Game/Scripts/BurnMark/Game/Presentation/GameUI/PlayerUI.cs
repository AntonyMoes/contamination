using System;
using System.Collections.Generic;
using _Game.Scripts.BaseUI;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel.User;
using TMPro;
using UnityEngine;

namespace _Game.Scripts.BurnMark.Game.Presentation.GameUI {
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

        [SerializeField] private AttackPreview _attackPreview;
        public AttackPreview AttackPreview => _attackPreview;

        [SerializeField] private EntityPanel _entityPanel;
        public EntityPanel EntityPanel => _entityPanel;

        [SerializeField] private Tooltip _tooltip;
        public Tooltip Tooltip => _tooltip;

        private ISet<int> _supportedPlayers;
        private GameDataReadAPI _readAPI;
        private GameDataEventsAPI _eventsAPI;
        private Action _onPlayerClosedGame;
        private int _currentPlayerId;

        public void Initialize(ISet<int> supportedPlayers, GameDataEventsAPI eventsAPI, Action endTurn, Action testEndGame, Action onPlayerClosedGame) {
            _supportedPlayers = supportedPlayers;
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
            _entityPanel.SetReadAPI(readAPI);
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

        private void OnTurnChanged(IReadOnlyUser _, IReadOnlyUser newPlayer) {
            var color = Game.Entities.Utils.GetInReadOnlyOwner<PlayerData>(newPlayer.Id, _readAPI)!.Data.Color;
            _currentPlayer.text = $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{newPlayer.Name}</color>";

            _turn.text = _readAPI.CurrentTurn.ToString();
            _endTurn.Enabled = _supportedPlayers.Contains(newPlayer.Id);
            _testEndGame.Enabled = _supportedPlayers.Contains(newPlayer.Id);
            _currentPlayerId = newPlayer.Id;

            var resourceComponent = Game.Entities.Utils.GetInReadOnlyOwner<ResourceData>(newPlayer.Id, _readAPI);
            OnResourceUpdate(null, resourceComponent);
        }

        private void OnResourceUpdate(ResourceData? _, IReadOnlyComponent<ResourceData> newData) {
            var owner = newData.ReadOnlyEntity.GetOwnerId();
            if (owner != _currentPlayerId) {
                return;
            }

            _moneyCounter.text = newData.Data.Money.ToString();
            _metalCounter.text = newData.Data.Metal.ToString();
        }
    }
}