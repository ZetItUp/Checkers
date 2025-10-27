using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Models
{
    public abstract class Piece
    {
        public PieceColor Color { get; init; }

        public Position Position { get; internal set; }

        public bool IsKing { get; protected set; }

        public Piece(PieceColor color, Position position)
        {
            Color = color;

            Position = position;
        }

        public abstract List<Position> GetValidMoves(IBoard board);
        public abstract Piece Clone();
        
    }
}