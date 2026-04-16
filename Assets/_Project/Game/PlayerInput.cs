using UnityEngine;
using ChessGame.Core;
using ChessGame.Gameplay;
using ChessGame.Game;

namespace ChessGame.Input
{
    public class PlayerInput : MonoBehaviour
    {
        public bool InputEnabled { get; set; } = true;

        private Piece _selectedPiece;
        private Camera _mainCamera;
        private GameController _controller;
        private ChessBoard _board;

        void Start()
        {
            _mainCamera = Camera.main;
            _controller = GetComponent<GameController>();
            _board = GetComponent<ChessBoard>();
        }

        void Update()
        {
            if (!InputEnabled) return;
            if (_controller != null && _controller.GameOver) return;
            if (_controller != null && _controller.CurrentTurn != GameSide.Red) return;

            var ai = GetComponent<AIOpponent>();
            if (ai != null && ai.IsThinking) return;

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Vector3 worldPos = _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                Vector2 rayOrigin = new Vector2(worldPos.x, worldPos.y);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero);

                if (hit.collider != null)
                {
                    Piece clickedPiece = hit.collider.GetComponent<Piece>();
                    if (clickedPiece != null)
                        OnPieceClicked(clickedPiece);
                }
                else if (_selectedPiece != null)
                {
                    int tx = Mathf.RoundToInt(worldPos.x);
                    int ty = Mathf.RoundToInt(worldPos.y);
                    TryMoveSelected(tx, ty);
                }
            }
        }

        private void OnPieceClicked(Piece piece)
        {
            if (_selectedPiece == null)
            {
                if (piece.Side == GameSide.Red)
                    SelectPiece(piece);
            }
            else
            {
                if (piece == _selectedPiece)
                {
                    Deselect();
                    return;
                }

                if (piece.Side == _selectedPiece.Side)
                    SelectPiece(piece);
                else
                    TryMoveSelected(piece.BoardX, piece.BoardY);
            }
        }

        public void SelectPiece(Piece piece)
        {
            if (_selectedPiece != null) _selectedPiece.SetHighlight(false);
            _selectedPiece = piece;
            _selectedPiece.SetHighlight(true);
        }

        public void Deselect()
        {
            if (_selectedPiece != null) _selectedPiece.SetHighlight(false);
            _selectedPiece = null;
        }

        private void TryMoveSelected(int tx, int ty)
        {
            if (_selectedPiece == null || _board == null) return;
            if (RuleEngine.IsValidMove(_board, _selectedPiece, tx, ty))
            {
                _board.MovePiece(_selectedPiece, tx, ty);
                Deselect();
                _controller?.EndPlayerTurn();
            }
        }
    }
}
