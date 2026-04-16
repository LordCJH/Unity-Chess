namespace ChessGame.Core
{
    public interface IChessPiece
    {
        PieceType Type { get; }
        GameSide Side { get; }
        int BoardX { get; }
        int BoardY { get; }
        bool IsAlive { get; }
    }
}
