﻿using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.Commands;

namespace _Game.Scripts.BurnMark.Game.Commands {
    public abstract class GameCommand : NetSerializedGameCommand {
        protected sealed override void PerformDoOnData(IGameAPI api) {
            PerformDoOnAPI((GameDataAPI) api);
        }

        protected abstract void PerformDoOnAPI(GameDataAPI api);
    }
}