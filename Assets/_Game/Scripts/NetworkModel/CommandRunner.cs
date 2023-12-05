using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.NetworkModel.Commands;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.NetworkModel {
    public class CommandRunner : ICommandSynchronizer, IDisposable {
        private readonly List<ICommandGenerator> _generators = new List<ICommandGenerator>();
        private readonly List<ICommandPresenter> _presenters = new List<ICommandPresenter>();
        private readonly Queue<GameCommand> _commandQueue = new Queue<GameCommand>();
        private readonly UpdatedValue<int> _queueSize = new UpdatedValue<int>();
        private readonly IGameAPI _api;

        private IInitialCommandGenerator _initialGenerator;
        private bool _initialCommandGenerated;
        private bool _isCommandRunning;

        public CommandRunner(IGameAPI api) {
            _api = api;
        }

        public void RegisterGenerator(ICommandGenerator generator) {
            generator.OnCommandGenerated.Subscribe(OnCommandGenerated);
            generator.SetReadAPI(_api);
            _generators.Add(generator);
        }

        public void RegisterInitialGenerator(IInitialCommandGenerator generator) {
            generator.OnCommandGenerated.Subscribe(OnInitialCommandGenerated);
            _initialGenerator = generator;
        }

        public void RegisterPresenter(ICommandPresenter presenter) {
            presenter.SetReadAPI(_api);
            _presenters.Add(presenter);
        }

        private void OnInitialCommandGenerated(GameCommand command) {
            _initialGenerator.OnCommandGenerated.Unsubscribe(OnCommandGenerated);

            if (_initialCommandGenerated) {
                throw new Exception("Initial command already generated!");
            }

            _initialCommandGenerated = true;

            RunCommand(command, _initialGenerator.OnInitialCommandFinished);
        }

        private void OnCommandGenerated(GameCommand command) {
            if (!_initialCommandGenerated) {
                throw new Exception("Initial command not generated!");
            }

            if (_isCommandRunning) {
                _commandQueue.Enqueue(command);
                _queueSize.Value++;
                return;
            }

            RunCommand(command);
        }

        private void RunCommand(GameCommand command, Action onCommandFinished = null, bool fromQueue = false) {
            Debug.Log($"Running command {command}");
            _isCommandRunning = true;
            var presentProcess = new SerialProcess();
            _presenters
                .Select(p => p.PresentCommand(command))
                .ForEach(presentProcess.Add);

            presentProcess.Run(() => {
                command.ProvideDataApi(_api);
                command.Do();
                _isCommandRunning = false;
                Debug.Log($"Finished command {command}");

                if (fromQueue) {
                    _queueSize.Value--;
                }

                onCommandFinished?.Invoke();

                TryRunNextCommand();
            });

            void TryRunNextCommand() {
                if (_commandQueue.Count == 0) {
                    return;
                }

                var newCommand = _commandQueue.Dequeue();
                RunCommand(newCommand, fromQueue: true);
            }
        }

        public void WaitForAllCommandsFinished(Action onDone) {
            _queueSize.WaitFor(0, onDone);
        }

        public void Dispose() {
            foreach (var generator in _generators) {
                generator.OnCommandGenerated.Unsubscribe(OnCommandGenerated);
            }
        }
    }
}
