using System.Collections.Generic;
using UnityEngine;
using ChessGame.Core;
using ChessGame.Gameplay;

namespace ChessGame.AI
{
    public static class AIEngine
    {
        public static ChessMove FindBestMove(ChessBoard board, GameSide aiSide, AIDifficulty difficulty)
        {
            int depth = (int)difficulty;
            var moves = board.GetAllValidMoves(aiSide);

            if (moves.Count == 0)
                return new ChessMove(-1, -1, -1, -1);

            Shuffle(moves);

            ChessMove bestMove = moves[0];
            float bestScore = float.NegativeInfinity;

            foreach (var move in moves)
            {
                var captured = board.SimulateMove(move);
                float score = Minimax(board, depth - 1, false, aiSide, float.NegativeInfinity, float.PositiveInfinity);
                board.UndoMove(move, captured);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private static float Minimax(ChessBoard board, int depth, bool isMaximizing, GameSide aiSide, float alpha, float beta)
        {
            if (depth == 0 || !board.HasGeneral(GameSide.Red) || !board.HasGeneral(GameSide.Black))
                return BoardEvaluator.Evaluate(board, aiSide);

            GameSide currentSide = isMaximizing ? aiSide : (aiSide == GameSide.Red ? GameSide.Black : GameSide.Red);
            var moves = board.GetAllValidMoves(currentSide);

            if (moves.Count == 0)
                return isMaximizing ? -99999f : 99999f;

            if (isMaximizing)
            {
                float maxEval = float.NegativeInfinity;
                foreach (var move in moves)
                {
                    var captured = board.SimulateMove(move);
                    float eval = Minimax(board, depth - 1, false, aiSide, alpha, beta);
                    board.UndoMove(move, captured);
                    maxEval = Mathf.Max(maxEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                    if (beta <= alpha) break;
                }
                return maxEval;
            }
            else
            {
                float minEval = float.PositiveInfinity;
                foreach (var move in moves)
                {
                    var captured = board.SimulateMove(move);
                    float eval = Minimax(board, depth - 1, true, aiSide, alpha, beta);
                    board.UndoMove(move, captured);
                    minEval = Mathf.Min(minEval, eval);
                    beta = Mathf.Min(beta, eval);
                    if (beta <= alpha) break;
                }
                return minEval;
            }
        }

        private static void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int r = Random.Range(i, n);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }
    }
}
