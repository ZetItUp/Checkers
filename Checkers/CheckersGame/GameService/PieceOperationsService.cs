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

        /// <summary>
        /// Konstruktor för PieceOperationsService
        /// </summary>
        /// <param name="ruleSet">IRuleRet</param>
        /// <param name="moveValidator">IMoveValidator</param>
        public PieceOperationsService(IRuleSet ruleSet, IMoveValidator moveValidator)
        {
            _ruleSet = ruleSet;
            _moveValidator = moveValidator;
        }

        /// <summary>
        /// Hanter vad som händer vid en capture
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="board">IBoard</param>
        /// <returns>Piece? that was captured</returns>
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

        /// <summary>
        /// Promote:a en pjäs till kung
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="piece">Piece</param>
        /// <param name="board"></param>
        public void PromoteToKing(Position position, Piece piece, IBoard board)
        {
            board.RemovePiece(position);
            var kingPiece = new KingPiece(piece.Color, position);
            board.PlacePiece(kingPiece, position);
        }

        /// <summary>
        /// Är en pjäs på en position för befordran
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="color">PieceColor</param>
        /// <returns>True if can be promoted</returns>
        public bool IsPromotionPosition(Position position, PieceColor color)
        {
            if (color == PieceColor.Light && position.Row == 0)
                return true;

            if (color == PieceColor.Dark && position.Row == _ruleSet.BoardSize - 1)
                return true;

            return false;
        }
    }
}