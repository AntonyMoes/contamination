using System;
using System.Linq;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.BurnMark.Game.Entities;
using _Game.Scripts.ModelV4;
using _Game.Scripts.ModelV4.ECS;
using _Game.Scripts.NetworkModel;
using GeneralUtils;
using GameCommand = _Game.Scripts.NetworkModel.Commands.GameCommand;

namespace _Game.Scripts.BurnMark.Game {
    public class GameEndChecker : ICommandGenerator {
        private readonly Action<GameCommand> _onCommandGenerated;
        public Event<GameCommand> OnCommandGenerated { get; }

        private readonly GameDataEventsAPI _eventsAPI;
        private GameDataReadAPI _readAPI;

        public GameEndChecker(GameDataEventsAPI eventsAPI) {
            OnCommandGenerated = new Event<GameCommand>(out _onCommandGenerated);

            _eventsAPI = eventsAPI;
            _eventsAPI.OnEntityDestroyed.Subscribe(OnEntityDestroyed);
        }

        public void SetReadAPI(IGameReadAPI api) {
            _readAPI = (GameDataReadAPI) api;
        }

        private void OnEntityDestroyed(Entity entity) {
            if (!Base.IsBase(entity)) {
                return;
            }

            var remainingBaseOwners = _readAPI.Entities.Values
                .Where(Base.IsBase)
                .Select(e => e.GetOwnerId())
                .Where(id => id != null)
                .Select(id => id.Value)
                .ToHashSet();
            if (remainingBaseOwners.Count != 1) {
                return;
            }

            _onCommandGenerated(new TestEndGameCommand {
                Winner = remainingBaseOwners.First()
            });
        }
    }
}