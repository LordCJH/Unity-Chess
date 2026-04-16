namespace ChessGame.Core
{
    public interface IChessBoard
    {
        IChessPiece GetPiece(int x, int y);
    }
}
