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
            
            int direction = (Color == PieceColor.Black) ? -1 : 1;
            
            Position forwardMoveLeft = new Position(Position.Row + direction, Position.Column - 1);
            Position forwardMoveRight = new Position(Position.Row + direction, Position.Column + 1);

            if (forwardMoveLeft.IsValid(board.Size))
            {
                forwardMoves.Add(forwardMoveLeft);
            }

            if (forwardMoveRight.IsValid(board.Size))
            {
                forwardMoves.Add(forwardMoveRight);
            }

            return forwardMoves;
        }

        private List<Position> GetCaptureMoves(Board board)
        {
            var captureMoves = new List<Position>();
            
            int direction = (Color == PieceColor.Black) ? -1 : 1;
            
            Position captureMoveLeft = new Position(Position.Row + direction, Position.Column - 1);
            Position captureMoveRight = new Position(Position.Row + direction, Position.Column + 1);

            if (captureMoveLeft.IsValid(board.Size))
            {
                captureMoves.Add(captureMoveLeft);
            }

            if (captureMoveRight.IsValid(board.Size))
            {
                captureMoves.Add(captureMoveRight);
            }

            return captureMoves;
        }
    }
}