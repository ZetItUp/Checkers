using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public class KingPiece : Piece
    {
        public KingPiece(PieceColor color, Position position) : base(color, position)
        {
        }

        public override List<Position> GetValidMoves(Board board)
        {
            return new List<Position>();
        }

        public override Piece Clone()
        {
            return new KingPiece(Color, Position);
        }

        private List<Position> GetAllDirectionMoves(Board board)
        {
            return new List<Position>();
        }

        private List<Position> GetAllDirectionCaptures(Board board)
        {
            return new List<Position>();
        }

    }
}