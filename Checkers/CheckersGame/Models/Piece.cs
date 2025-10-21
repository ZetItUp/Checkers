using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public abstract class Piece
    {
        public PieceColor Color { get; set; }

        public CurrentPosition Position { get; set; }

        public readonly bool IsKing;

        public Piece(PieceColor color, CurrentPosition position)
        {
            this.Color = color;

            this.Position = position;
        }
        
        
    }
}