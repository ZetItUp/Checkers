using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public class KingPiece : Piece
    {
        public KingPiece(PieceColor color, CurrentPosition position) : base(color, position)
        {
            this.Color = color;

            this.Position = position;
        }
    }
}