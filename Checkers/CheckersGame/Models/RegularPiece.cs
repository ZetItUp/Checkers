using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public class RegularPiece: Piece
    {
        public RegularPiece(PieceColor color, Position position) : base(color, position)
        {
            this.Color = color;

            this.Position = position;
        }

        public override Piece Clone()
        {
            var clone = new RegularPiece(Color, Position);

            return clone;
        }

        public override List<Position> GetValidMoves(Board board)
        {
            var validMoves = new List<Position>();
            validMoves.AddRange(GetForwardMoves(board));
            validMoves.AddRange(GetCaptureMoves(board));
            return validMoves;
        }


        private List<Position> GetForwardMoves(Board board)
        {
            var forwardMoves = new List<Position>();

            int directionBlack = (Color == PieceColor.Black) ? -1 : 1;
            
            int directionRed = (Color == PieceColor.Red) ? 1 : -1;
            
            Position forwardMoveLeftBlack = new Position(Position.Row + directionBlack, Position.Column - 1);
            Position forwardMoveRightBlack = new Position(Position.Row + directionBlack, Position.Column + 1);
            Position forwardMoveLeftRed = new Position(Position.Row + directionRed, Position.Column + 1);
            Position forwardMoveRightRed = new Position(Position.Row + directionRed, Position.Column - 1);

            if (forwardMoveLeftBlack.IsValid(board.Size))
            {
                forwardMoves.Add(forwardMoveLeftBlack);
            }

            if (forwardMoveRightBlack.IsValid(board.Size))
            {
                forwardMoves.Add(forwardMoveRightBlack);
            }

            if (forwardMoveLeftRed.IsValid(board.Size))
            {
                forwardMoves.Add(forwardMoveLeftRed);
            }

            if (forwardMoveRightRed.IsValid(board.Size))
            {
                forwardMoves.Add(forwardMoveRightRed);
            }

            return forwardMoves;
        }

        private List<Position> GetCaptureMoves(Board board)
        {
            var captureMoves = new List<Position>();
            
            int directionBlack = (Color == PieceColor.Black) ? -2 : 2;
            int directionRed = (Color == PieceColor.Black) ? 2 : -2;
            
            Position captureMoveLeftBlack = new Position(Position.Row + directionBlack, Position.Column - 2);
            Position captureMoveRightBlack = new Position(Position.Row + directionBlack, Position.Column + 2);
            Position captureMoveLeftRed = new Position(Position.Row + directionRed, Position.Column + 2);
            Position captureMoveRightRed = new Position(Position.Row + directionRed, Position.Column - 2);

            if (captureMoveLeftBlack.IsValid(board.Size))
            {
                captureMoves.Add(captureMoveLeftBlack);
            }

            if (captureMoveRightBlack.IsValid(board.Size))
            {
                captureMoves.Add(captureMoveRightBlack);
            }

            if (captureMoveLeftRed.IsValid(board.Size))
            {
                captureMoves.Add(captureMoveLeftRed);
            }

            if (captureMoveRightRed.IsValid(board.Size))
            {
                captureMoves.Add(captureMoveRightRed);
            }

            return captureMoves;
        }
    }
}