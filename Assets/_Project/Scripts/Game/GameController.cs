using UnityEngine;
using UnityEngine.EventSystems;
using ChessGame.Core;
using ChessGame.Gameplay;
using ChessGame.UI;
using ChessGame.Input;
using ChessGame.AI;

namespace ChessGame.Game
{
    [RequireComponent(typeof(PieceFactory))]
    public class GameController : MonoBehaviour
    {
        [SerializeField] private RectTransform _boardRect;

        private MenuView _menuView;
        private GameOverView _gameOverView;
        private ButtonsView _buttonsView;

        private PieceFactory _pieceFactory;
        private ChessBoard _board;
        private PlayerInput _playerInput;
        private AIOpponent _aiOpponent;

        public GameSide CurrentTurn { get; private set; } = GameSide.Red;
        public bool GameOver { get; private set; }

        void Awake()
        {
            SetupCamera();
            EnsureEventSystem();

            _pieceFactory = GetComponent<PieceFactory>();
            _board = gameObject.AddComponent<ChessBoard>();
            _playerInput = gameObject.AddComponent<PlayerInput>();
            _aiOpponent = gameObject.AddComponent<AIOpponent>();

            _boardRect = ResolveBoardRect();
            _pieceFactory.SetBoardRoot(_boardRect);
            _playerInput.SetBoardRect(_boardRect);

            _board.Initialize(_pieceFactory);
            _aiOpponent.Initialize(_board);
        }

        void Start()
        {
            ResolveViews();
            BindEvents();
            ShowMenu();
            _playerInput.InputEnabled = false;

        }

        private void ResolveViews()
        {
            if (_menuView == null)
            {
                var go = GameObject.Find("MenuCanvas");
                if (go != null) _menuView = go.GetComponent<MenuView>();
            }
            if (_gameOverView == null)
            {
                var views = Resources.FindObjectsOfTypeAll<GameOverView>();
                if (views.Length > 0) _gameOverView = views[0];
                // GameOverView resolved
            }
            if (_buttonsView == null)
            {
                var views = Resources.FindObjectsOfTypeAll<ButtonsView>();
                if (views.Length > 0) _buttonsView = views[0];
                // ButtonsView resolved
            }
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.transform.position = new Vector3(4f, 4.5f, -10f);
                cam.orthographic = true;
                cam.orthographicSize = 6f;
            }
        }

        private void EnsureEventSystem()
        {
            if (GameObject.Find("EventSystem") == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }
        }

        private RectTransform ResolveBoardRect()
        {
            if (_boardRect != null)
                return _boardRect;

            var boardObject = GameObject.Find("MainUI/ChessBoard") ?? GameObject.Find("ChessBoard");
            return boardObject != null ? boardObject.GetComponent<RectTransform>() : null;
        }

        private void BindEvents()
        {
            if (_menuView != null)
            {
                _menuView.OnStartClicked += StartGame;
                _menuView.OnQuitClicked += QuitGame;
            }
            if (_buttonsView != null)
            {
                _buttonsView.OnRegretClicked += RegretMove;
                _buttonsView.OnSurrenderClicked += Surrender;
            }
            if (_gameOverView != null)
            {
                _gameOverView.OnRestartClicked += ReturnToMenu;
                _gameOverView.OnMenuClicked += ReturnToMenu;
            }
        }

        private void ShowMenu()
        {
            if (_menuView != null) _menuView.Show();
            if (_gameOverView != null) _gameOverView.Hide();
            if (_buttonsView != null) _buttonsView.Hide();
        }

        private void StartGame(AIDifficulty difficulty)
        {
            _aiOpponent.SetDifficulty(difficulty);
            if (_menuView != null) _menuView.Hide();
            if (_buttonsView != null) _buttonsView.Show();
            _board.SpawnAllPieces();

            CurrentTurn = GameSide.Red;
            GameOver = false;
            _playerInput.InputEnabled = true;
            _playerInput.Deselect();
        }

        public void EndPlayerTurn()
        {
            if (GameOver) return;
            CurrentTurn = GameSide.Black;
            CheckGameOver();
            if (!GameOver)
                _aiOpponent.RequestMove(this);
        }

        public void RegretMove()
        {
            if (GameOver || _board == null || !_board.CanUndo) return;

            // Prevent regret while AI is thinking
            if (_aiOpponent != null && _aiOpponent.IsThinking) return;

            // Undo the most recent move (AI's move if it just moved)
            var record1 = _board.UndoLastMove();
            if (record1 == null) return;

            // If the undone move was AI's, also undo player's previous move to restore fair state
            if (record1.MovedPieceSide == GameSide.Black && _board.CanUndo)
            {
                var record2 = _board.UndoLastMove();
                if (record2 != null)
                {
                    CurrentTurn = GameSide.Red;
                }
            }
            else if (record1.MovedPieceSide == GameSide.Red)
            {
                CurrentTurn = GameSide.Red;
            }

            _playerInput.InputEnabled = true;
            _playerInput.Deselect();
        }

        public void EndAITurn()
        {
            if (GameOver) return;
            CurrentTurn = GameSide.Red;
            CheckGameOver();
            _playerInput.InputEnabled = !GameOver;
        }

        private void CheckGameOver()
        {
            if (_board == null) return;

            string winner = null;
            if (!_board.HasGeneral(GameSide.Red))
            {
                winner = "黑方";
                GameOver = true;
            }
            else if (!_board.HasGeneral(GameSide.Black))
            {
                winner = "红方";
                GameOver = true;
            }

            if (GameOver && winner != null)
            {
                if (_buttonsView != null) _buttonsView.Hide();
                _playerInput.InputEnabled = false;
                if (_gameOverView != null) _gameOverView.Show($"{winner}获胜！");
            }
        }

        private void Surrender()
        {
            if (GameOver) return;
            GameOver = true;
            if (_buttonsView != null) _buttonsView.Hide();
            _playerInput.InputEnabled = false;
            if (_gameOverView != null) _gameOverView.Show("你认输了！");
        }

        private void ReturnToMenu()
        {
            if (_gameOverView != null) _gameOverView.Hide();
            if (_buttonsView != null) _buttonsView.Hide();
            _board.ClearBoard();
            CurrentTurn = GameSide.Red;
            GameOver = false;
            _playerInput.InputEnabled = false;
            _playerInput.Deselect();
            ShowMenu();
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
