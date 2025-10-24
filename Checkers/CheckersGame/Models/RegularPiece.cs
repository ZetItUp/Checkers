using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Models
{
    public class RegularPiece: Piece
    {
        public RegularPiece(PieceColor color, Position position) : base(color, position)
        {
            this.Color = color;

            this.Position = position;

            this.IsKing = false;
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

            // Svarta pjäser rör sig NER (rad ökar), Röda pjäser rör sig UPP (rad minskar)
            int direction = (Color == PieceColor.Black) ? 1 : -1;

            // Beräkna de två diagonala fram-dragen
            Position forwardLeft = new Position(Position.Row + direction, Position.Column - 1);
            Position forwardRight = new Position(Position.Row + direction, Position.Column + 1);

            if (forwardLeft.IsValid(board.Size))
            {
                forwardMoves.Add(forwardLeft);
            }

            if (forwardRight.IsValid(board.Size))
            {
                forwardMoves.Add(forwardRight);
            }

            return forwardMoves;
        }

        private List<Position> GetCaptureMoves(Board board)
        {
            var captureMoves = new List<Position>();

            // Svarta pjäser tar NER (rad ökar med 2), Röda pjäser tar UPP (rad minskar med 2)
            int direction = (Color == PieceColor.Black) ? 2 : -2;

            // Beräkna de två diagonala landningspositionerna för capture (hoppa över motståndarens pjäs)
            Position captureLeft = new Position(Position.Row + direction, Position.Column - 2);
            Position captureRight = new Position(Position.Row + direction, Position.Column + 2);

            if (captureLeft.IsValid(board.Size))
            {
                captureMoves.Add(captureLeft);
            }

            if (captureRight.IsValid(board.Size))
            {
                captureMoves.Add(captureRight);
            }

            return captureMoves;
        }
    }
}