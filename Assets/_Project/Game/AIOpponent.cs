using System.Collections;
using UnityEngine;
using ChessGame.Core;
using ChessGame.Gameplay;
using ChessGame.AI;

namespace ChessGame.Game
{
    public class AIOpponent : MonoBehaviour
    {
        [SerializeField] private float _thinkingDelay = 0.5f;

        private ChessBoard _board;
        private AIDifficulty _difficulty = AIDifficulty.Normal;
        private bool _isThinking;

        public bool IsThinking => _isThinking;

        public void Initialize(ChessBoard board)
        {
            _board = board;
        }

        public void SetDifficulty(AIDifficulty difficulty)
        {
            _difficulty = difficulty;
        }

        public void RequestMove(GameController controller)
        {
            if (_isThinking) return;
            StartCoroutine(ThinkAndMove(controller));
        }

        private IEnumerator ThinkAndMove(GameController controller)
        {
            _isThinking = true;
            yield return new WaitForSeconds(_thinkingDelay);

            if (_board != null)
            {
                var move = AIEngine.FindBestMove(_board, GameSide.Black, _difficulty);
                if (move.FromX >= 0)
                {
                    var piece = _board.GetPiece(move.FromX, move.FromY);
                    if (piece != null && piece.Side == GameSide.Black)
                    {
                        _board.MovePiece(piece, move.ToX, move.ToY);
                        controller?.EndAITurn();
                    }
                }
            }

            _isThinking = false;
        }
    }
}
