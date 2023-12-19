using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.ModelV4.ECS.Systems;
using _Game.Scripts.Network;
using _Game.Scripts.NetworkModel;
using _Game.Scripts.NetworkModel.Commands;
using _Game.Scripts.NetworkModel.Network;
using _Game.Scripts.NetworkModel.User;

namespace _Game.Scripts.ModelV4 {
    public class Game /*: IDisposable*/ {
        private readonly InitialCommandGenerator _initialCommandGenerator;
        private readonly IUser[] _users;
        private readonly IDisposable[] _otherDisposables;
        private readonly CommandRunner _runner;

        private readonly TurnController _turnController;
        private readonly ECS.ECS _ecs;
        private readonly GameDataAPI _api;
        public GameDataReadAPI ReadAPI => _api;
        public readonly GameDataEventsAPI EventsAPI;

        private Game(GameCommand initialCommand, IEnumerable<IUser> users, params IDisposable[] otherDisposables) {
            _users = users.ToArray();
            _otherDisposables = otherDisposables;
            _turnController = new TurnController(_users);

            _ecs = new ModelV4.ECS.ECS();

            _api = new GameDataAPI(_ecs, _turnController);
            EventsAPI = new GameDataEventsAPI(_ecs, _turnController);

            _runner = new CommandRunner(_api);
            _turnController.SetCommandSynchronizer(_runner);
            _runner.RegisterGenerator(_turnController);

            _initialCommandGenerator = new InitialCommandGenerator(initialCommand, _turnController);
            _runner.RegisterInitialGenerator(_initialCommandGenerator);
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

        public void RegisterSystem(ECS.Systems.System system) {
            system.Initialize(_api, _ecs.Entities.Values);

            EventsAPI.OnEntityCreated.Subscribe(system.TryAddEntity);
            EventsAPI.OnEntityDestroyed.Subscribe(system.TryRemoveEntity);

            if (system is IStartTurnSystem startTurnSystem) {
                EventsAPI.OnTurnChanged.Subscribe(Subscriber);

                void Subscriber(IReadOnlyUser _, IReadOnlyUser user) {
                    if (user == null) {
                        return;
                    }

                    startTurnSystem.OnStartTurn(user.Id, _turnController.CurrentTurn);
                }
            }
        }

        public void Dispose() {
            _runner?.Dispose();

            _ecs.Clear();

            foreach (var user in _users) {
                user.Dispose();
            }

            foreach (var disposable in _otherDisposables) {
                disposable.Dispose();
            }
        }

        public static Game CreateLocal(GameCommand initialCommand, IEnumerable<(int, string, ICommandGenerator)> usersData) {
            var users = usersData.Select(tuple => new LocalUser(tuple.Item1, tuple.Item2, tuple.Item3));
            return new Game(initialCommand, users);
        }

        public static Game StartClient(GameConfigurationMessage message, IPeer serverPeer, ICommandGenerator userCommandGenerator) {
            var gameClient = new GameClient(serverPeer);
            var users = Enumerable.Range(0, message.Players.Length)
                .Select(idx => {
                    var id = message.Players[idx];
                    var name = message.PlayerNames[idx];
                    return id == message.CurrenUser
                        ? new LocalNetworkUser(new LocalUser(id, name, userCommandGenerator), gameClient)
                        : (IUser) new RemoteNetworkUser(id, name, gameClient);
                });
            return new Game(message.InitialCommand, users, gameClient);
        }

        public static Game StartServer(GameConfigurationMessage message, IEnumerable<IPeer> clientPeers, ICommandGenerator userCommandGenerator) {
            var gameServer = new GameServer(clientPeers);
            var users = Enumerable.Range(0, message.Players.Length)
                .Select(idx => {
                    var id = message.Players[idx];
                    var name = message.PlayerNames[idx];
                    return id == message.CurrenUser
                        ? new LocalNetworkUser(new LocalUser(id, name, userCommandGenerator), gameServer)
                        : (IUser) new RemoteNetworkUser(id, name, gameServer);
                });
            return new Game(message.InitialCommand, users, gameServer);
        }

        public static Game StartStandaloneServer(GameConfigurationMessage message, IEnumerable<IPeer> clientPeers) {
            var gameServer = new GameServer(clientPeers);
            var users = Enumerable.Range(0, message.Players.Length)
                .Select(idx => new RemoteNetworkUser(message.Players[idx], message.PlayerNames[idx], gameServer));
            return new Game(message.InitialCommand, users, gameServer);
        }
    }
}
