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

        public Position Position { get; set; }

        public readonly bool IsKing;

        public Piece(PieceColor color, Position position)
        {
            Color = color;

            Position = position;
        }
       
        public abstract List<Position> GetValidMoves(Board board);
        public abstract Piece Clone();
        
    }
}