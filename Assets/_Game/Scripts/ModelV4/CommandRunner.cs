using System;
using System.Collections.Generic;
using System.Linq;
using GeneralUtils;
using GeneralUtils.Processes;
using UnityEngine;

namespace _Game.Scripts.ModelV4 {
    public class CommandRunner : ICommandSynchronizer, IDisposable {
        private readonly List<ICommandGenerator> _generators = new List<ICommandGenerator>();
        private readonly List<ICommandPresenter> _presenters = new List<ICommandPresenter>();
        private readonly Queue<GameCommand> _commandQueue = new Queue<GameCommand>();
        private readonly UpdatedValue<int> _queueSize = new UpdatedValue<int>();
        private readonly GameDataAPI _api;
        private readonly GameDataReadAPI _readAPI;

        private bool _isCommandRunning;

        public CommandRunner(GameDataAPI api, GameDataReadAPI readAPI) {
            _api = api;
            _readAPI = readAPI;
        }

        public void RegisterGenerator(ICommandGenerator generator) {
            generator.OnCommandGenerated.Subscribe(OnCommandGenerated);
            generator.SetReadAPI(_readAPI);
            _generators.Add(generator);
        }

        public void RegisterPresenter(ICommandPresenter presenter) {
            presenter.SetReadAPI(_readAPI);
            _presenters.Add(presenter);
        }

        private void OnCommandGenerated(GameCommand command) {
            if (_isCommandRunning) {
                _commandQueue.Enqueue(command);
                _queueSize.Value++;
                return;
            }

            RunCommand(command);
        }

        private void RunCommand(GameCommand command, bool fromQueue = false) {
            Debug.Log($"Running command {command.Serialize()}");
            _isCommandRunning = true;
            var presentProcess = new SerialProcess();
            _presenters
                .Select(p => p.PresentCommand(command))
                .Where(process => process != null)
                .ForEach(presentProcess.Add);

            presentProcess.Run(() => {
                command.ProvideDataApi(_api);
                command.Do();
                _isCommandRunning = false;
                Debug.Log($"Finished command {command.Serialize()}");

                if (fromQueue) {
                    _queueSize.Value--;
                }

                TryRunNextCommand();
            });

            void TryRunNextCommand() {
                if (_commandQueue.Count == 0) {
                    return;
                }

                var newCommand = _commandQueue.Dequeue();
                RunCommand(newCommand, true);
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
