using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.Network;
using _Game.Scripts.ModelV4.User;
using _Game.Scripts.Network;

namespace _Game.Scripts.ModelV4 {
    public class Game : IDisposable {
        private readonly InitialCommandGenerator _initialCommandGenerator;
        private readonly CommandRunner _runner;

        public readonly GameDataEventsAPI EventsAPI;

        private Game(GameCommand initialCommand, IEnumerable<IUser> users) {
            var turnController = new TurnController(users);

            var ecs = new ECS.ECS();

            var api = new GameDataAPI(ecs, turnController);
            var readApi = new GameDataReadAPI(ecs, turnController);
            EventsAPI = new GameDataEventsAPI(ecs, turnController);

            _runner = new CommandRunner(api, readApi);
            turnController.SetCommandSynchronizer(_runner);
            _runner.RegisterGenerator(turnController);

            _initialCommandGenerator = new InitialCommandGenerator(initialCommand);
            _runner.RegisterGenerator(_initialCommandGenerator);
        }

        public void Start() {
            _initialCommandGenerator.TriggerInitialCommand();
        }

        public void RegisterGenerator(ICommandGenerator generator) {
            _runner.RegisterGenerator(generator);
        }

        public void RegisterPresenter(ICommandPresenter presenter) {
            _runner.RegisterPresenter(presenter);
        }

        public void Dispose() {
            _runner?.Dispose();
        }

        public static Game CreateLocal(GameCommand initialCommand, IEnumerable<string> userNames, ICommandGenerator userCommandGenerator) {
            var users = userNames.Select((name, idx) => new LocalUser(idx, name, userCommandGenerator));
            return new Game(initialCommand, users);
        }

        public static Game StartClient(GameConfigurationMessage message, IPeer serverPeer, ICommandGenerator userCommandGenerator) {
            var gameClient = new GameClient(serverPeer);
            var users = Enumerable.Range(0, message.UserSequence.Length)
                .Select(idx => {
                    var id = message.UserSequence[idx];
                    var name = message.UserNames[idx];
                    return id == message.CurrenUser
                        ? new LocalNetworkUser(new LocalUser(id, name, userCommandGenerator), gameClient)
                        : (IUser) new RemoteNetworkUser(id, name, gameClient);
                });
            return new Game(message.InitialCommand, users);
        }

        public static Game StartServer(GameConfigurationMessage message, IEnumerable<IPeer> clientPeers, ICommandGenerator userCommandGenerator) {
            var gameServer = new GameServer(clientPeers);
            var users = Enumerable.Range(0, message.UserSequence.Length)
                .Select(idx => {
                    var id = message.UserSequence[idx];
                    var name = message.UserNames[idx];
                    return id == message.CurrenUser
                        ? new LocalNetworkUser(new LocalUser(id, name, userCommandGenerator), gameServer)
                        : (IUser) new RemoteNetworkUser(id, name, gameServer);
                });
            return new Game(message.InitialCommand, users);
        }

        public static Game StartStandaloneServer(GameConfigurationMessage message, IEnumerable<IPeer> clientPeers) {
            var gameServer = new GameServer(clientPeers);
            var users = Enumerable.Range(0, message.UserSequence.Length)
                .Select(idx => new RemoteNetworkUser(message.UserSequence[idx], message.UserNames[idx], gameServer));
            return new Game(message.InitialCommand, users);
        }
    }
}
