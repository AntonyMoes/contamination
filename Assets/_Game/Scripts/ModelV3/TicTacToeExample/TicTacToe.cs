using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts.ModelV3.TicTacToeExample {
    public class TicTacToe : MonoBehaviour {
        [SerializeField] private Button _startButton;
        [SerializeField] private TicTacToeInteractor _interactor;
        
        [SerializeField] private int _size;

        private readonly TicTacToeState _state = new TicTacToeState();
        private SimpleRunner _runner;


        private void Start() {
            _startButton.onClick.AddListener(StartGame);
        }

        private void StartGame() {
            _state.Reset(_size);
            _interactor.InitializeField(_state);

            var proxyCreator = new ProxyCreator(_state, new[] {_interactor});
            var markOrder = new[] {TicTacToeState.EMark.X, TicTacToeState.EMark.O};
            var mechanics = new TicTacToeMechanics(_state, markOrder, _interactor.SetCurrentMark, OnWin);

            _runner = new SimpleRunner();
            _runner.SetCreators(new []{ proxyCreator });
            _runner.SetReactors(new ICommandReactor[]{ _interactor, mechanics });

            void OnWin(TicTacToeState.EMark mark) {
                proxyCreator.Disable();
                Debug.Log($"{mark} won!");
            }
        }
    }
}
