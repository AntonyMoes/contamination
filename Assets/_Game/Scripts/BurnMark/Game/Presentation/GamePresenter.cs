﻿using System;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Data.Components;
using _Game.Scripts.BurnMark.Game.Mechanics;
using _Game.Scripts.BurnMark.Game.Presentation.GameField;
using _Game.Scripts.BurnMark.Game.Presentation.GameField.FieldActions;
using _Game.Scripts.BurnMark.Game.Presentation.GameUI;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using GeneralUtils.Processes;
using JetBrains.Annotations;
using UnityEngine;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game.Presentation {
    public class GamePresenter : ICommandPresenter, IDisposable, IFieldActionUIPresenter {
        private readonly ProxyCommandGenerator _proxy;
        private readonly int _player;
        private readonly PlayerUI _playerUI;
        private readonly FieldPresenter _fieldPresenter;

        public GamePresenter(ProxyCommandGenerator proxy, int player, PlayerUI playerUI, GameDataEventsAPI eventsAPI,
            Action onPlayerClosedGame, Func<IFieldActionUIPresenter, FieldPresenter> fieldPresenterCreator) {
            _proxy = proxy;
            _player = player;
            _playerUI = playerUI;
            _fieldPresenter = fieldPresenterCreator(this);
            _playerUI.Initialize(_player, eventsAPI, EndTurn, EndGame, onPlayerClosedGame);
            _playerUI.gameObject.SetActive(true);
            _fieldPresenter.OnCommandGenerated.Subscribe(_proxy.GenerateCommand);
            _fieldPresenter.OnEntitySelected.Subscribe(OnEntitySelected);
        }

        public void Dispose() {
            _playerUI.gameObject.SetActive(false);
            _fieldPresenter.OnCommandGenerated.Unsubscribe(_proxy.GenerateCommand);
            _fieldPresenter.OnEntitySelected.Unsubscribe(OnEntitySelected);
            _fieldPresenter.Dispose();
        }

        public void SetReadAPI(IGameReadAPI api) {
            _playerUI.SetReadAPI((GameDataReadAPI) api);
            _fieldPresenter.SetReadAPI(api);
        }

        private void EndTurn() {
            _proxy.GenerateCommand(new EndTurnCommand());
        }

        private void EndGame() {
            _proxy.GenerateCommand(new TestEndGameCommand());
        }

        public Process PresentCommand(GameCommand generatedCommand) {
            Debug.Log($"PRESENTING {generatedCommand}");

            return ParallelProcess.From(PerformPresent(generatedCommand),
                _fieldPresenter.PresentCommand(generatedCommand));
            
        }

        private Process PerformPresent(GameCommand generatedCommand) {
            switch (generatedCommand) {
                case TestEndGameCommand endGameCommand:
                    return new SyncProcess(() => _playerUI.OnGameEnded(endGameCommand.Winner == _player));
                default:
                    return new DummyProcess();
            }
        }

        private void OnEntitySelected([CanBeNull] IReadOnlyEntity entity) {
            if (entity != null) {
                _playerUI.EntityPanel.Initialize(entity);
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
            // var config = target.GetReadOnlyComponent<UnitData>() is {} unitData
            //     ? (FieldEntityConfig) unitData.Data.Config
            //     : target.GetReadOnlyComponent<FieldObjectData>()!.Data.Config;
            var damage = Attacking.CalculateDamage(attack, health);
            _playerUI.AttackPreview.Initialize(config.Icon, config.Name, health, damage);
        }

        public void HideAttackPreview() {
            _playerUI.AttackPreview.gameObject.SetActive(false);
        }
    }
}