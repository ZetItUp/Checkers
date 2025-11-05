using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.Models;

namespace Checkers.CheckersGame.Validation
{
    /// <summary>
    /// Interface för Move Validator
    /// </summary>
    public interface IMoveValidator
    {
        bool ValidateMove(Position from, Position to, IBoard board, Player player);
        Position? GetCapturedPosition(Position from, Position to);
        bool IsCapture(Position from, Position to);
        bool HasValidMoves(Player player, IBoard board);
    }
}