﻿using System;
using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;
using GeneralUtils.Processes;

namespace _Game.Scripts.NetworkModel.User {
    public abstract class User : IUser {
        private readonly Action<bool> _onUserTurnToggled;

        public int Id { get; }
        public string Name { get; }

        protected User(int id, string name) {
            Id = id;
            Name = name;
            
            OnUserTurnToggled = new Event<bool>(out _onUserTurnToggled);
        }

        public abstract void SetReadAPI(IGameReadAPI api);
        public abstract Event<GameCommand> OnCommandGenerated { get; }

        public Process EndTurn() {
            _onUserTurnToggled(false);
            return PerformEndTurn();
        }
        protected abstract Process PerformEndTurn();
        public void StartTurn() {
            _onUserTurnToggled(true);
        }

        public Event<bool> OnUserTurnToggled { get; }

        public abstract void Dispose();
    }
}
