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
            return new RegularPiece(this.Color, new Position(this.Position.Row, this.Position.Column));
        }

        public override List<Position> GetValidMoves(Board board)
        {
            return new List<Position>();
        }
    }
}