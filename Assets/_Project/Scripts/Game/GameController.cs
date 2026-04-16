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

        private PieceFactory _pieceFactory;
        private ChessBoard _board;
        private PlayerInput _playerInput;
        private AIOpponent _aiOpponent;
        private MenuView _menuView;
        private GameOverView _gameOverView;
        private SurrenderView _surrenderView;

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
            _menuView = gameObject.AddComponent<MenuView>();
            _gameOverView = gameObject.AddComponent<GameOverView>();
            _surrenderView = gameObject.AddComponent<SurrenderView>();

            _boardRect = ResolveBoardRect();
            _pieceFactory.SetBoardRoot(_boardRect);
            _playerInput.SetBoardRect(_boardRect);

            _board.Initialize(_pieceFactory);
            _aiOpponent.Initialize(_board);

            _menuView.BuildUI();

            BindEvents();

            ShowMenu();
            _playerInput.InputEnabled = false;
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
            if (FindObjectOfType<EventSystem>() == null)
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
            _menuView.OnStartClicked += StartGame;
            _menuView.OnQuitClicked += QuitGame;
            _surrenderView.OnSurrenderClicked += Surrender;
            _gameOverView.OnRestartClicked += ReturnToMenu;
            _gameOverView.OnMenuClicked += ReturnToMenu;
        }

        private void ShowMenu()
        {
            _menuView.Show();
            _gameOverView.Hide();
            _surrenderView.Hide();
        }

        private void StartGame(AIDifficulty difficulty)
        {
            _aiOpponent.SetDifficulty(difficulty);
            _menuView.Hide();
            _surrenderView.Show();
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
                _surrenderView.Hide();
                _playerInput.InputEnabled = false;
                _gameOverView.Show($"{winner}获胜！");
            }
        }

        private void Surrender()
        {
            if (GameOver) return;
            GameOver = true;
            _surrenderView.Hide();
            _playerInput.InputEnabled = false;
            ReturnToMenu();
        }

        private void ReturnToMenu()
        {
            _gameOverView.Hide();
            _surrenderView.Hide();
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
