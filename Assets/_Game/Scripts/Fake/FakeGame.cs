using System;
using System.Collections.Generic;
using _Game.Scripts.BurnMark.Game.Commands;
using _Game.Scripts.ModelV4;
using _Game.Scripts.NetworkModel;
using LiteNetLib.Utils;

namespace _Game.Scripts.Fake {
    public static class FakeGame {
        public static Game Create(Action<GameDataAPI> initialCommand, ICommandGenerator userCommandGenerator, IEnumerable<string> userNames = null) {
            return Game.CreateLocal(new InitialCommand(initialCommand), userNames ?? new[] { "" }, userCommandGenerator);
        }

        private class InitialCommand : GameCommand {
            private readonly Action<GameDataAPI> _initialCommand;

            public InitialCommand(Action<GameDataAPI> initialCommand) {
                _initialCommand = initialCommand;
            }
            
            protected override void SerializeContents(NetDataWriter writer) { }

            protected override void DeserializeContents(NetDataReader reader) { }

            protected override void PerformDoOnAPI(GameDataAPI api) {
                _initialCommand(api);
            }
        }
    }
}