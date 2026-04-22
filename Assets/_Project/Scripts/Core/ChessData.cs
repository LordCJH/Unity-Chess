namespace ChessGame.Core
{
    public enum PieceType
    {
        General, Advisor, Elephant, Chariot, Horse, Cannon, Soldier
    }

    public enum GameSide
    {
        Red, Black
    }

    public readonly struct ChessMove
    {
        public readonly int FromX;
        public readonly int FromY;
        public readonly int ToX;
        public readonly int ToY;

        public ChessMove(int fromX, int fromY, int toX, int toY)
        {
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
        }

        public override string ToString()
        {
            return $"({FromX},{FromY})->({ToX},{ToY})";
        }
    }

    public class MoveRecord
    {
        public readonly PieceType MovedPieceType;
        public readonly GameSide MovedPieceSide;
        public readonly int FromX;
        public readonly int FromY;
        public readonly int ToX;
        public readonly int ToY;
        public readonly PieceType? CapturedPieceType;
        public readonly GameSide? CapturedPieceSide;

        public MoveRecord(PieceType movedType, GameSide movedSide, int fromX, int fromY, int toX, int toY, PieceType? capturedType, GameSide? capturedSide)
        {
            MovedPieceType = movedType;
            MovedPieceSide = movedSide;
            FromX = fromX;
            FromY = fromY;
            ToX = toX;
            ToY = toY;
            CapturedPieceType = capturedType;
            CapturedPieceSide = capturedSide;
        }
    }
}
