using System.Collections.Generic;
using ChessGame.Core;

namespace ChessGame.Gameplay
{
    public static class MoveGenerator
    {
        private static readonly (int dx, int dy)[] ElephantOffsets = {
            (2, 2), (2, -2), (-2, 2), (-2, -2)
        };

        private static readonly (int dx, int dy)[] HorseOffsets = {
            (1, 2), (1, -2), (-1, 2), (-1, -2),
            (2, 1), (2, -1), (-2, 1), (-2, -1)
        };

        public static List<ChessMove> GetAllValidMoves(IChessBoard board, Piece[,] pieces, GameSide side)
        {
            var moves = new List<ChessMove>(60);
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var p = pieces[x, y];
                    if (p == null || p.Side != side) continue;
                    GenerateMovesForPiece(board, p, moves);
                }
            }
            return moves;
        }

        private static void GenerateMovesForPiece(IChessBoard board, Piece piece, List<ChessMove> moves)
        {
            int x = piece.BoardX;
            int y = piece.BoardY;

            switch (piece.Type)
            {
                case PieceType.General:
                case PieceType.Advisor:
                    GeneratePalaceMoves(board, piece, moves);
                    break;
                case PieceType.Elephant:
                    foreach (var (dx, dy) in ElephantOffsets)
                        TryAddMove(board, piece, x + dx, y + dy, moves);
                    break;
                case PieceType.Horse:
                    foreach (var (dx, dy) in HorseOffsets)
                        TryAddMove(board, piece, x + dx, y + dy, moves);
                    break;
                case PieceType.Soldier:
                    int dyStep = piece.Side == GameSide.Red ? 1 : -1;
                    TryAddMove(board, piece, x, y + dyStep, moves);
                    TryAddMove(board, piece, x + 1, y, moves);
                    TryAddMove(board, piece, x - 1, y, moves);
                    break;
                default: // Chariot & Cannon
                    for (int i = 0; i < 9; i++)
                        if (i != x) TryAddMove(board, piece, i, y, moves);
                    for (int i = 0; i < 10; i++)
                        if (i != y) TryAddMove(board, piece, x, i, moves);
                    break;
            }
        }

        private static void GeneratePalaceMoves(IChessBoard board, Piece piece, List<ChessMove> moves)
        {
            int x = piece.BoardX;
            int y = piece.BoardY;
            for (int tx = 3; tx <= 5; tx++)
                for (int ty = 0; ty < 10; ty++)
                    if (tx != x || ty != y)
                        TryAddMove(board, piece, tx, ty, moves);
        }

        private static void TryAddMove(IChessBoard board, Piece piece, int tx, int ty, List<ChessMove> moves)
        {
            if (RuleEngine.IsValidMove(board, piece, tx, ty))
                moves.Add(new ChessMove(piece.BoardX, piece.BoardY, tx, ty));
        }
    }
}
