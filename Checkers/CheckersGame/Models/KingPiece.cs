using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Models
{
    public class KingPiece : Piece
    {
        private static readonly int[] DirectionRows = { -1, 1 };
        private static readonly int[] DirectionCols = { -1, 1 };

        public KingPiece(PieceColor color, Position position) : base(color, position)
        {
            IsKing = true;
        }

        public override List<Position> GetValidMoves(IBoard board)
        {
            var validMoves = new List<Position>();
            validMoves.AddRange(GetAllDirectionMoves(board));
            validMoves.AddRange(GetAllDirectionCaptures(board));
            return validMoves;
        }

        public override Piece Clone()
        {
            return new KingPiece(Color, Position);
        }

        private List<Position> GetAllDirectionMoves(IBoard board)
        {
            var forwardMoves = new List<Position>();

            foreach (var rowDirection in DirectionRows)
            {
                foreach (var colDirection in DirectionCols)
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

        private List<Position> GetAllDirectionCaptures(IBoard board)
        {
            var captureMoves = new List<Position>();

            foreach (var rowDirection in DirectionRows)
            {
                foreach (var colDirection in DirectionCols)
                {
                    Position captureMove = new Position(Position.Row + rowDirection, Position.Column + colDirection);
                    Position landingSpot = new Position(captureMove.Row + rowDirection, captureMove.Column + colDirection);
                    if (captureMove.IsValid(board.Size) && landingSpot.IsValid(board.Size))
                    {
                        captureMoves.Add(landingSpot);
                    }
                }
            }

            return captureMoves;
        }

    }
}