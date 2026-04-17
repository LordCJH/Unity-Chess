using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core;

namespace ChessGame.Gameplay
{
    public class ChessBoard : MonoBehaviour, IChessBoard
    {
        private readonly Piece[,] _pieces = new Piece[9, 10];
        private PieceFactory _pieceFactory;
        private Piece _lastMoved;

        public void Initialize(PieceFactory factory)
        {
            _pieceFactory = factory;
            ClearBoard();
        }

        public void ResetBoard()
        {
            ClearBoard();
            SpawnAllPieces();
        }

        public void ClearBoard()
        {
            ClearPieces();
            _lastMoved = null;
        }

        public void SpawnAllPieces()
        {
            SpawnAll();
        }

        private void ClearPieces()
        {
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (_pieces[x, y] != null)
                    {
                        if (_pieces[x, y].gameObject != null)
                            Destroy(_pieces[x, y].gameObject);
                        _pieces[x, y] = null;
                    }
                }
            }
        }

        private void SpawnAll()
        {
            Spawn(PieceType.Chariot, GameSide.Red, 0, 0);
            Spawn(PieceType.Horse, GameSide.Red, 1, 0);
            Spawn(PieceType.Elephant, GameSide.Red, 2, 0);
            Spawn(PieceType.Advisor, GameSide.Red, 3, 0);
            Spawn(PieceType.General, GameSide.Red, 4, 0);
            Spawn(PieceType.Advisor, GameSide.Red, 5, 0);
            Spawn(PieceType.Elephant, GameSide.Red, 6, 0);
            Spawn(PieceType.Horse, GameSide.Red, 7, 0);
            Spawn(PieceType.Chariot, GameSide.Red, 8, 0);
            Spawn(PieceType.Cannon, GameSide.Red, 1, 2);
            Spawn(PieceType.Cannon, GameSide.Red, 7, 2);
            for (int i = 0; i < 5; i++)
                Spawn(PieceType.Soldier, GameSide.Red, i * 2, 3);

            Spawn(PieceType.Chariot, GameSide.Black, 0, 9);
            Spawn(PieceType.Horse, GameSide.Black, 1, 9);
            Spawn(PieceType.Elephant, GameSide.Black, 2, 9);
            Spawn(PieceType.Advisor, GameSide.Black, 3, 9);
            Spawn(PieceType.General, GameSide.Black, 4, 9);
            Spawn(PieceType.Advisor, GameSide.Black, 5, 9);
            Spawn(PieceType.Elephant, GameSide.Black, 6, 9);
            Spawn(PieceType.Horse, GameSide.Black, 7, 9);
            Spawn(PieceType.Chariot, GameSide.Black, 8, 9);
            Spawn(PieceType.Cannon, GameSide.Black, 1, 7);
            Spawn(PieceType.Cannon, GameSide.Black, 7, 7);
            for (int i = 0; i < 5; i++)
                Spawn(PieceType.Soldier, GameSide.Black, i * 2, 6);
        }

        private void Spawn(PieceType type, GameSide side, int x, int y)
        {
            if (_pieceFactory == null) return;
            var piece = _pieceFactory.CreatePiece(type, side, x, y);
            _pieces[x, y] = piece;
        }

        public Piece GetPiece(int x, int y)
        {
            if (x < 0 || x > 8 || y < 0 || y > 9) return null;
            return _pieces[x, y];
        }

        IChessPiece IChessBoard.GetPiece(int x, int y) => GetPiece(x, y);

        public void MovePiece(Piece piece, int toX, int toY)
        {
            _pieces[piece.BoardX, piece.BoardY] = null;
            var target = _pieces[toX, toY];
            if (target != null)
            {
                target.IsAlive = false;
                Destroy(target.gameObject);
            }
            _pieces[toX, toY] = piece;
            piece.BoardX = toX;
            piece.BoardY = toY;
            piece.UpdatePosition();
            UpdateMoveBorder(piece);
        }

        private void UpdateMoveBorder(Piece piece)
        {
            if (_lastMoved != null && _lastMoved != piece)
                _lastMoved.SetMoveBorder(false);

            piece.SetMoveBorder(true);
            _lastMoved = piece;
        }

        public List<ChessMove> GetAllValidMoves(GameSide side)
        {
            return MoveGenerator.GetAllValidMoves(this, _pieces, side);
        }

        public Piece SimulateMove(ChessMove move)
        {
            var piece = _pieces[move.FromX, move.FromY];
            var captured = _pieces[move.ToX, move.ToY];
            _pieces[move.FromX, move.FromY] = null;
            _pieces[move.ToX, move.ToY] = piece;
            piece.BoardX = move.ToX;
            piece.BoardY = move.ToY;
            if (captured != null) captured.IsAlive = false;
            return captured;
        }

        public void UndoMove(ChessMove move, Piece captured)
        {
            var piece = _pieces[move.ToX, move.ToY];
            _pieces[move.ToX, move.ToY] = captured;
            _pieces[move.FromX, move.FromY] = piece;
            piece.BoardX = move.FromX;
            piece.BoardY = move.FromY;
            if (captured != null) captured.IsAlive = true;
        }

        public bool HasGeneral(GameSide side)
        {
            for (int x = 0; x < 9; x++)
                for (int y = 0; y < 10; y++)
                {
                    var p = _pieces[x, y];
                    if (p != null && p.Side == side && p.Type == PieceType.General)
                        return true;
                }
            return false;
        }
    }
}
