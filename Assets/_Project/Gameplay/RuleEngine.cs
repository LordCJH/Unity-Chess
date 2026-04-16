using UnityEngine;
using ChessGame.Core;

namespace ChessGame.Gameplay
{
    public static class RuleEngine
    {
        public static bool IsValidMove(IChessBoard board, IChessPiece piece, int tx, int ty)
        {
            if (tx < 0 || tx > 8 || ty < 0 || ty > 9) return false;
            var target = board.GetPiece(tx, ty);
            if (target != null && target.Side == piece.Side) return false;

            int dx = tx - piece.BoardX;
            int dy = ty - piece.BoardY;
            int adx = Mathf.Abs(dx);
            int ady = Mathf.Abs(dy);

            return piece.Type switch
            {
                PieceType.General => IsGeneralMoveValid(piece, tx, ty, adx, ady),
                PieceType.Advisor => IsAdvisorMoveValid(piece, tx, ty, adx, ady),
                PieceType.Elephant => IsElephantMoveValid(board, piece, tx, ty, dx, dy, adx, ady),
                PieceType.Chariot => IsChariotMoveValid(board, piece, tx, ty),
                PieceType.Horse => IsHorseMoveValid(board, piece, tx, ty, adx, ady),
                PieceType.Cannon => IsCannonMoveValid(board, piece, tx, ty, target),
                PieceType.Soldier => IsSoldierMoveValid(piece, tx, ty, dx, dy, adx, ady),
                _ => false,
            };
        }

        private static bool InPalace(int x, int y, GameSide side)
        {
            if (x < 3 || x > 5) return false;
            return side == GameSide.Red ? y is >= 0 and <= 2 : y is >= 7 and <= 9;
        }

        private static bool IsGeneralMoveValid(IChessPiece piece, int tx, int ty, int adx, int ady)
        {
            return InPalace(tx, ty, piece.Side) && adx + ady == 1;
        }

        private static bool IsAdvisorMoveValid(IChessPiece piece, int tx, int ty, int adx, int ady)
        {
            return InPalace(tx, ty, piece.Side) && adx == 1 && ady == 1;
        }

        private static bool IsElephantMoveValid(IChessBoard board, IChessPiece piece, int tx, int ty, int dx, int dy, int adx, int ady)
        {
            if (adx != 2 || ady != 2) return false;
            bool redSide = piece.Side == GameSide.Red;
            if (redSide && ty > 4) return false;
            if (!redSide && ty < 5) return false;

            int eyeX = piece.BoardX + dx / 2;
            int eyeY = piece.BoardY + dy / 2;
            return board.GetPiece(eyeX, eyeY) == null;
        }

        private static bool IsChariotMoveValid(IChessBoard board, IChessPiece piece, int tx, int ty)
        {
            if (piece.BoardX != tx && piece.BoardY != ty) return false;
            return CountPiecesBetween(board, piece.BoardX, piece.BoardY, tx, ty) == 0;
        }

        private static bool IsHorseMoveValid(IChessBoard board, IChessPiece piece, int tx, int ty, int adx, int ady)
        {
            if (!((adx == 1 && ady == 2) || (adx == 2 && ady == 1))) return false;

            int legX, legY;
            if (adx == 2)
            {
                legX = piece.BoardX + (tx - piece.BoardX) / 2;
                legY = piece.BoardY;
            }
            else
            {
                legX = piece.BoardX;
                legY = piece.BoardY + (ty - piece.BoardY) / 2;
            }
            return board.GetPiece(legX, legY) == null;
        }

        private static bool IsCannonMoveValid(IChessBoard board, IChessPiece piece, int tx, int ty, IChessPiece target)
        {
            if (piece.BoardX != tx && piece.BoardY != ty) return false;
            int count = CountPiecesBetween(board, piece.BoardX, piece.BoardY, tx, ty);
            return target == null ? count == 0 : count == 1;
        }

        private static bool IsSoldierMoveValid(IChessPiece piece, int tx, int ty, int dx, int dy, int adx, int ady)
        {
            if (adx + ady != 1) return false;
            bool crossedRiver = piece.Side == GameSide.Red ? piece.BoardY >= 5 : piece.BoardY <= 4;

            if (piece.Side == GameSide.Red)
            {
                if (dy < 0) return false;
                if (!crossedRiver && dx != 0) return false;
            }
            else
            {
                if (dy > 0) return false;
                if (!crossedRiver && dx != 0) return false;
            }
            return true;
        }

        private static int CountPiecesBetween(IChessBoard board, int x1, int y1, int x2, int y2)
        {
            int count = 0;
            if (x1 == x2)
            {
                int min = Mathf.Min(y1, y2);
                int max = Mathf.Max(y1, y2);
                for (int y = min + 1; y < max; y++)
                    if (board.GetPiece(x1, y) != null) count++;
            }
            else if (y1 == y2)
            {
                int min = Mathf.Min(x1, x2);
                int max = Mathf.Max(x1, x2);
                for (int x = min + 1; x < max; x++)
                    if (board.GetPiece(x, y1) != null) count++;
            }
            return count;
        }
    }
}
