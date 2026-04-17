using UnityEngine;
using UnityEngine.UI;
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
        private RectTransform _boardRect;
        private Canvas _boardCanvas;

        public void SetBoardRect(RectTransform boardRect)
        {
            _boardRect = boardRect;
            _boardCanvas = boardRect != null ? boardRect.GetComponentInParent<Canvas>() : null;
        }

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

            if (UnityEngine.Input.GetMouseButtonDown(0) && TryGetBoardCell(UnityEngine.Input.mousePosition, out int tx, out int ty))
            {
                Piece clickedPiece = _board != null ? _board.GetPiece(tx, ty) : null;
                if (clickedPiece != null)
                {
                    OnPieceClicked(clickedPiece);
                }
                else if (_selectedPiece != null)
                {
                    TryMoveSelected(tx, ty);
                }
            }
        }

        private bool TryGetBoardCell(Vector2 screenPoint, out int boardX, out int boardY)
        {
            boardX = -1;
            boardY = -1;
            if (_boardRect == null)
                return false;

            Camera eventCamera = _boardCanvas != null && _boardCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? _boardCanvas.worldCamera
                : null;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_boardRect, screenPoint, eventCamera, out var localPoint))
                return false;

            Rect rect = _boardRect.rect;
            float cellWidth = rect.width / 8f;
            float cellHeight = rect.height / 9f;
            var hitRect = new Rect(
                rect.xMin - cellWidth * 0.5f,
                rect.yMin - cellHeight * 0.5f,
                rect.width + cellWidth,
                rect.height + cellHeight);
            if (!hitRect.Contains(localPoint))
                return false;

            localPoint.x = Mathf.Clamp(localPoint.x, rect.xMin, rect.xMax);
            localPoint.y = Mathf.Clamp(localPoint.y, rect.yMin, rect.yMax);
            boardX = Mathf.RoundToInt(localPoint.x / cellWidth + 4f);
            boardY = Mathf.RoundToInt(localPoint.y / cellHeight + 4.5f);
            return boardX >= 0 && boardX <= 8 && boardY >= 0 && boardY <= 9;
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
