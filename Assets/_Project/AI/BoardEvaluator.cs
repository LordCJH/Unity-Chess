using UnityEngine;
using ChessGame.Core;
using ChessGame.Gameplay;

namespace ChessGame.AI
{
    public static class BoardEvaluator
    {
        public static float GetPieceValue(PieceType type)
        {
            return type switch
            {
                PieceType.General => 10000f,
                PieceType.Chariot => 900f,
                PieceType.Cannon => 450f,
                PieceType.Horse => 400f,
                PieceType.Advisor => 200f,
                PieceType.Elephant => 200f,
                PieceType.Soldier => 100f,
                _ => 0f,
            };
        }

        public static float Evaluate(ChessBoard board, GameSide aiSide)
        {
            float score = 0f;
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    var piece = board.GetPiece(x, y);
                    if (piece == null) continue;

                    float value = GetPieceValue(piece.Type);
                    value += GetPositionBonus(piece, x, y);

                    if (piece.Side == aiSide)
                        score += value;
                    else
                        score -= value;
                }
            }
            return score;
        }

        private static float GetPositionBonus(Piece piece, int x, int y)
        {
            float bonus = 0f;

            if (piece.Type == PieceType.Soldier)
            {
                bool crossedRiver = piece.Side == GameSide.Red ? y >= 5 : y <= 4;
                bool deepBehind = piece.Side == GameSide.Red ? y >= 7 : y <= 2;
                if (crossedRiver) bonus += 50f;
                if (deepBehind) bonus += 50f;
            }
            else if (piece.Type == PieceType.Chariot)
            {
                if (x >= 3 && x <= 5) bonus += 30f;
            }
            else if (piece.Type == PieceType.Horse)
            {
                // 马在开阔位置有加成
                if (x >= 2 && x <= 6 && y >= 2 && y <= 7)
                    bonus += 20f;
            }

            return bonus;
        }

    }
}
