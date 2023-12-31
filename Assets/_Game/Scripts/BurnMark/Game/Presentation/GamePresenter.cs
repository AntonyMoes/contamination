using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.BurnMark.Game.Presentation.GameUI;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.User;
using _Game.Scripts.Scheduling;
using GeneralUtils;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class GamePresenter : ICommandPresenter, IDisposable, IFieldActionUIPresenter, IFrameProcessor {
        private readonly LocalProxyCommandGenerator _localProxy;
        private readonly PlayerUI _playerUI;
        private readonly GameDataEventsAPI _eventsAPI;
        private readonly IScheduler _scheduler;
        private readonly PointerRaycastProvider _pointerRaycastProvider;
        private readonly TooltipProviderTracker _tooltipProviderTracker;
        private readonly ISet<int> _supportedPlayers;
        private readonly FieldPresenter _fieldPresenter;

        private GameDataReadAPI _readAPI;
        private int _currentPlayer;
        private ProxyCommandGenerator _proxy;

        public GamePresenter(LocalProxyCommandGenerator localProxy, PlayerUI playerUI, Camera uiCamera,
            GameDataEventsAPI eventsAPI, IScheduler scheduler, PointerRaycastProvider pointerRaycastProvider,
            Action onPlayerClosedGame, Func<IFieldActionUIPresenter, FieldPresenter> fieldPresenterCreator,
            IEnumerable<int> supportedPlayers) : this(playerUI, uiCamera, eventsAPI, scheduler, pointerRaycastProvider,
            onPlayerClosedGame, fieldPresenterCreator, supportedPlayers) {
            _localProxy = localProxy;
        }

        public GamePresenter(ProxyCommandGenerator proxy, PlayerUI playerUI, Camera uiCamera,
            GameDataEventsAPI eventsAPI, IScheduler scheduler, PointerRaycastProvider pointerRaycastProvider,
            Action onPlayerClosedGame, Func<IFieldActionUIPresenter, FieldPresenter> fieldPresenterCreator,
            IEnumerable<int> supportedPlayers) : this(playerUI, uiCamera, eventsAPI, scheduler, pointerRaycastProvider,
            onPlayerClosedGame, fieldPresenterCreator, supportedPlayers) {
            _proxy = proxy;
        }

        private GamePresenter(PlayerUI playerUI, Camera uiCamera, GameDataEventsAPI eventsAPI, IScheduler scheduler,
            PointerRaycastProvider pointerRaycastProvider, Action onPlayerClosedGame,
            Func<IFieldActionUIPresenter, FieldPresenter> fieldPresenterCreator, IEnumerable<int> supportedPlayers) {
            _playerUI = playerUI;
            _eventsAPI = eventsAPI;
            _scheduler = scheduler;
            _scheduler.RegisterFrameProcessor(this);
            _pointerRaycastProvider = pointerRaycastProvider;
            _tooltipProviderTracker = new TooltipProviderTracker(pointerRaycastProvider);
            _scheduler.RegisterFrameProcessor(_tooltipProviderTracker);
            _tooltipProviderTracker.TooltipProvider.Subscribe(OnTooltipProviderChange);
            _supportedPlayers = supportedPlayers.ToHashSet();
            _eventsAPI.OnTurnChanged.Subscribe(OnTurnChanged);
            _fieldPresenter = fieldPresenterCreator(this);
            _playerUI.Initialize(_supportedPlayers, eventsAPI, EndTurn, EndGame, onPlayerClosedGame);
            _playerUI.gameObject.SetActive(true);
            _playerUI.EntityPanel.OnBuild.Subscribe(OnBuild);
            _playerUI.EntityPanel.OnCancelBuild.Subscribe(OnCancelBuild);
            _fieldPresenter.OnCommandGenerated.Subscribe(OnFieldCommandGenerated);
            _fieldPresenter.OnEntitySelected.Subscribe(OnEntitySelected);
            _playerUI.Tooltip.Initialize(uiCamera);
        }

        public void Dispose() {
            _scheduler.UnregisterFrameProcessor(this);
            _scheduler.UnregisterFrameProcessor(_tooltipProviderTracker);
            _eventsAPI.OnTurnChanged.Unsubscribe(OnTurnChanged);
            _playerUI.gameObject.SetActive(false);
            _playerUI.EntityPanel.Clear();
            _fieldPresenter.OnCommandGenerated.Unsubscribe(OnFieldCommandGenerated);
            _fieldPresenter.OnEntitySelected.Unsubscribe(OnEntitySelected);
            _fieldPresenter.Dispose();
        }

        public void ProcessFrame(float deltaTime) {
            // var res = string.Join(",", _pointerRaycastProvider.RaycastResults.Select(res => {
            //     var tooltip = res.gameObject.GetComponentInParent<ITooltipProvider>() != null;
            //     return tooltip ? $"<color=greeen>{res.gameObject.name}</color>" : res.gameObject.name;
            // }));
            // if (!string.IsNullOrEmpty(res)) {
            //     Debug.Log(res);
            // }
        }

        public void SetReadAPI(IGameReadAPI api) {
            _readAPI = (GameDataReadAPI) api;
            _playerUI.SetReadAPI(_readAPI);
            _fieldPresenter.SetReadAPI(api);
        }

        private void OnTurnChanged([CanBeNull] IReadOnlyUser _, [CanBeNull] IReadOnlyUser player) {
            if (player == null) {
                return;
            }

            _currentPlayer = player.Id;
            if (_localProxy != null) {
                _proxy = _localProxy.Get(player.Id);
            }

            _fieldPresenter.SetPlayer(_supportedPlayers.Contains(player.Id) ? player.Id : (int?) null);
        }

        private void EndTurn() {
            _proxy.GenerateCommand(new EndTurnCommand());
        }

        private void EndGame() {
            _proxy.GenerateCommand(new TestEndGameCommand());
        }

        private void OnEntityCommandClicked(IReadOnlyEntity entity, GameCommand command) {
            if (CurrentSupported(entity.GetOwnerId())) {
                _proxy.GenerateCommand(command);
            }
        }

        private void OnBuild(int builderId, int order) {
            if (CurrentSupported(_readAPI.Entities[builderId].GetOwnerId())) {
                _proxy.GenerateCommand(new BuildUnitCommand {
                    BuilderId = builderId,
                    UnitConfigOrder = order
                });
            }
        }

        private void OnCancelBuild(int builderId, int queuePosition) {
            if (CurrentSupported(_readAPI.Entities[builderId].GetOwnerId())) {
                _proxy.GenerateCommand(new CancelBuildUnitCommand {
                    BuilderId = builderId,
                    QueuePosition = queuePosition
                });
            }
        }

        private void OnFieldCommandGenerated(GameCommand command) {
            _proxy.GenerateCommand(command);
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            Debug.Log($"PRESENTING {generatedCommand}");

            return ParallelProcess.From(PerformPresent(generatedCommand),
                _fieldPresenter.PresentCommand(generatedCommand));
            
        }

        private Process PerformPresent(GameCommand generatedCommand) {
            switch (generatedCommand) {
                case TestEndGameCommand endGameCommand:
                    return new SyncProcess(() => _playerUI.OnGameEnded(_supportedPlayers.Contains(endGameCommand.Winner)));
                default:
                    return new DummyProcess();
            }
        }

        private void OnEntitySelected([CanBeNull] IReadOnlyEntity entity) {
            if (entity != null) {
                _playerUI.EntityPanel.Initialize(entity, OnEntityCommandClicked);
                _playerUI.EntityPanel.gameObject.SetActive(true);
            } else {
                _playerUI.EntityPanel.gameObject.SetActive(false);
            }
        }

        public void ShowAttackPreview(IReadOnlyEntity attacker, IReadOnlyEntity target) {
            _playerUI.AttackPreview.gameObject.SetActive(true);
            var attack = attacker.GetReadOnlyComponent<AttackData>()!.Data;
            var health = target.GetReadOnlyComponent<HealthData>()!.Data;
            var config = target.TryGetFieldEntityConfig()!;
            var damage = Attacking.CalculateDamage(attack, health);
            _playerUI.AttackPreview.Initialize(config.Icon, config.Name, health, damage);
        }

        public void HideAttackPreview() {
            _playerUI.AttackPreview.gameObject.SetActive(false);
        }

        private bool CurrentSupported(int? player) {
            return player == _currentPlayer && _supportedPlayers.Contains(_currentPlayer);
        }

        private void OnTooltipProviderChange((ITooltipProvider, Vector2) pair) {
            var (tooltipProvider, screenPosition) = pair;
            if (tooltipProvider == null) {
                _playerUI.Tooltip.Hide();
            } else {
                _playerUI.Tooltip.Show(tooltipProvider, screenPosition);
            }
        }
    }
}