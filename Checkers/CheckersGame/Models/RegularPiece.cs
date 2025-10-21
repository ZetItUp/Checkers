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
    }
}