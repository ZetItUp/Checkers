using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.Validation;

namespace Checkers.CheckersGame.GameService
{
    /// Hanterar piece operationer som ta och befordra
    public class PieceOperationsService
    {
        private readonly IRuleSet _ruleSet;
        private readonly IMoveValidator _moveValidator;

        public PieceOperationsService(IRuleSet ruleSet, IMoveValidator moveValidator)
        {
            _ruleSet = ruleSet;
            _moveValidator = moveValidator;
        }

        
        public Piece? HandleCapture(Position from, Position to, IBoard board)
        {
            var capturedPosition = _moveValidator.GetCapturedPosition(from, to);
            if (capturedPosition.HasValue)
            {
                var capturedPiece = board.GetPiece(capturedPosition.Value);

                // Remove the piece only if it exists
                if (capturedPiece != null)
                {
                    board.RemovePiece(capturedPosition.Value);
                }

                return capturedPiece;
            }

            return null;
        }

        public void PromoteToKing(Position position, Piece piece, IBoard board)
        {
            board.RemovePiece(position);
            var kingPiece = new KingPiece(piece.Color, position);
            board.PlacePiece(kingPiece, position);
        }

        public bool IsPromotionPosition(Position position, PieceColor color)
        {
            if (color == PieceColor.Red && position.Row == 0)
                return true;

            if (color == PieceColor.Black && position.Row == _ruleSet.BoardSize - 1)
                return true;

            return false;
        }
    }
}