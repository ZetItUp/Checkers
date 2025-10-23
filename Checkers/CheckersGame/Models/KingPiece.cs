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

        int[] directionRows = { -1, 1 };
        int[] directionCols = { -1, 1 };

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
            var forwardMoves = new List<Position>();

            foreach (var rowDirection in directionRows)
            {
                foreach (var colDirection in directionCols)
                {
                    Position move = new Position(Position.Row + rowDirection, Position.Column + colDirection);

                    if (move.IsValid(board.Size))
                    {
                        forwardMoves.Add(move);
                    }
                }
            }

            return forwardMoves;
        }

        private List<Position> GetAllDirectionCaptures(Board board)
        {
            var captureMoves = new List<Position>();
            
            foreach (var rowDirection in directionRows)
            {
                foreach (var colDirection in directionCols)
                {
                    Position captureMove = new Position(Position.Row + rowDirection, Position.Column + colDirection);
                    Position landingSpot = new Position(captureMove.Row + rowDirection, captureMove.Column + colDirection);
                    if (captureMove.IsValid(board.Size) && landingSpot.IsValid(board.Size))
                    {
                        captureMoves.Add(landingSpot);
                        board.RemovePiece(captureMove);
                    }
                }
            }

            return captureMoves;
        }

    }
}