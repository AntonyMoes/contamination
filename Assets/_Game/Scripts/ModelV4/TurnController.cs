using System;
using System.Collections.Generic;
using _Game.Scripts.ModelV4.User;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.ModelV4 {
    public class TurnController : ICommandGenerator {
        private readonly List<IUser> _userSequence;
        private readonly Action<GameCommand> _onCommandGenerated;
        private int _currentUserIndex;
        private IUser CurrentModifiableUser => _userSequence[_currentUserIndex];

        public IReadOnlyCollection<IReadOnlyUser> UserSequence => _userSequence;
        public IReadOnlyUser CurrentUser => CurrentModifiableUser;

        private readonly Action<IReadOnlyUser, IReadOnlyUser> _onTurnChanged;
        public readonly Event<IReadOnlyUser, IReadOnlyUser> OnTurnChanged;

        public TurnController(IEnumerable<IUser> userSequence) {
            _userSequence = new List<IUser>(userSequence);

            OnTurnChanged = new Event<IReadOnlyUser, IReadOnlyUser>(out _onTurnChanged);
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);
            CurrentModifiableUser.OnCommandGenerated.Subscribe(_onCommandGenerated);
        }

        public Event<GameCommand> OnCommandGenerated { get; }
        public void SetReadAPI(GameDataReadAPI api) {
            foreach (var user in _userSequence) {
                user.SetReadAPI(api);
            }
        }

        // kinda hacky: synchronously switch current user, but continue to listen for their commands until sync ends
        public void EndTurn() {
            var currentUser = CurrentModifiableUser;
            _currentUserIndex = GetNextUserIndex(_currentUserIndex, _userSequence.Count);
            var newCurrentUser = CurrentModifiableUser;

            var endTurnProcess = new SerialProcess();
            endTurnProcess.Add(currentUser.EndTurn());
            endTurnProcess.Add(new SyncProcess(() => {
                currentUser.OnCommandGenerated.Unsubscribe(_onCommandGenerated);
                newCurrentUser.OnCommandGenerated.Subscribe(_onCommandGenerated);
                _onTurnChanged.Invoke(currentUser, newCurrentUser);
                newCurrentUser.StartTurn();
            }));
        }

        public void UndoEndTurn() {
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
