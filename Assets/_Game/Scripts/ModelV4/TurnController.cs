﻿using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.User;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.ModelV4 {
    public class TurnController : ICommandGenerator {
        private readonly List<IUser> _userSequence;
        private readonly Action<GameCommand> _onCommandGenerated;
        private int _currentUserIndex;
        private IUser CurrentModifiableUser => _currentUserIndex >= 0 && _currentUserIndex < _userSequence.Count 
            ? _userSequence[_currentUserIndex]
            : null;

        public IReadOnlyCollection<IReadOnlyUser> UserSequence => _userSequence;
        public IReadOnlyUser CurrentUser => CurrentModifiableUser;

        private readonly Action<IReadOnlyUser, IReadOnlyUser> _onTurnChanged;
        public readonly Event<IReadOnlyUser, IReadOnlyUser> OnTurnChanged;
        private ICommandSynchronizer _commandSynchronizer;

        public TurnController(IEnumerable<IUser> userSequence) {
            _userSequence = new List<IUser>(userSequence);

            OnTurnChanged = new Event<IReadOnlyUser, IReadOnlyUser>(out _onTurnChanged);
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            CurrentModifiableUser?.OnCommandGenerated.Subscribe(OnUserCommandGenerated);
        }

        private void OnUserCommandGenerated(GameCommand command) {
            _onCommandGenerated(command);
        }

        public void SetCommandSynchronizer(ICommandSynchronizer synchronizer) {
            _commandSynchronizer = synchronizer;
        }

        public Event<GameCommand> OnCommandGenerated { get; }
        public void SetReadAPI(GameDataReadAPI api) {
            foreach (var user in _userSequence) {
                user.SetReadAPI(api);
            }
        }

        // Kinda hacky: synchronously switch current user, but continue to listen for their commands until sync ends.
        // TODO: stop processing local commands in user on end turn but continue processing network commands until synced???
        public void EndTurn(bool endGame = false) {
            var currentUser = CurrentModifiableUser;
            _currentUserIndex = !endGame ? GetNextUserIndex(_currentUserIndex, _userSequence.Count) : -1;
            var newCurrentUser = CurrentModifiableUser;

            var endTurnProcess = new SerialProcess();
            endTurnProcess.Add(new SyncProcess(() => Debug.Log($"Ending turn for {currentUser?.Id}")));
            endTurnProcess.Add(currentUser?.EndTurn() ?? new DummyProcess());
            endTurnProcess.Add(new SyncProcess(() => Debug.Log($"Ended turn for {currentUser?.Id}")));
            endTurnProcess.Add(new AsyncProcess(_commandSynchronizer.WaitForAllCommandsFinished));
            endTurnProcess.Add(new SyncProcess(() => Debug.Log("Synchronized commands")));
            endTurnProcess.Add(new SyncProcess(() => {
                // TODO: account for possible different synchronization times; command can already start pouring into the system
                currentUser?.OnCommandGenerated.Unsubscribe(OnUserCommandGenerated);
                newCurrentUser?.OnCommandGenerated.Subscribe(OnUserCommandGenerated);
                _onTurnChanged.Invoke(currentUser, newCurrentUser);
                newCurrentUser?.StartTurn();
            }));
            endTurnProcess.Run();
        }

        public void UndoEndTurn(bool gameEnded = false) {
            // TODO ??????????
        }

        private static int GetNextUserIndex(int currentUserIndex, int userCount) {
            return (currentUserIndex + 1) % userCount;
        }

        private static int GetPreviousUserIndex(int currentUserIndex, int userCount) {
            return (currentUserIndex + userCount - 1) % userCount;
        }
    }
}
