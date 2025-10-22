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
            return new List<Position>();
        }


        private List<Position> GetForwardMoves(Board board)
        {
            return new List<Position>();
        }

        private List<Position> GetCaptureMoves(Board board)
        {
            return new List<Position>();
        }
    }
}