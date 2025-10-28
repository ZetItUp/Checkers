using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.Models;

namespace Checkers.CheckersGame.History
{
    public class GameHistory : IGameHistory
    {
        private readonly IBoard _initialBoard;
        private readonly List<Move> _moves;

        public GameHistory(IBoard initialBoard)
        {
            _initialBoard = initialBoard;
            _moves = new List<Move>();
        }

        public void RecordMove(Move move)
        {
            move.MoveNumber = _moves.Count + 1;
            _moves.Add(move);
        }

        public List<Move> GetAllMoves()
        {
            return new List<Move>(_moves);
        }

        public bool Undo(IBoard board)
        {
            if (_moves.Count == 0)
                return false;

            //ta bort det sista draget
            _moves.RemoveAt(_moves.Count - 1);

            ResetBoard(board);

            foreach (var move in _moves)
            {
                ApplyMove(board, move);
            }
            return true;
        }

        public void Clear()
        {
            _moves.Clear();    
        }

        private void ResetBoard(IBoard board)
        {
            // Ta bara bort pjäser från rutor som faktiskt har pjäser
            for (int row = 0; row < board.Size; row++)
            {
                for (int col = 0; col < board.Size; col++)
                {
                    var position = new Position(row, col);
                    if (board.GetPiece(position) != null)
                    {
                        board.RemovePiece(position);
                    }
                }
            }

            var initalPieces = _initialBoard.GetAllPieces();
            foreach (var piece in  initalPieces){
                var clonedPiece = piece.Clone();
                board.PlacePiece(clonedPiece, clonedPiece.Position);
            }
        }

        private void ApplyMove(IBoard board, Move move)
        {
            board.MovePiece(move.From, move.To);
            
            if (move.CapturedPiece != null)
            {
                var captureRow = (move.From.Row + move.To.Row) / 2;
                var captureCol = (move.From.Column + move.To.Column) / 2;
                board.RemovePiece(new Position(captureRow, captureCol));
            }
            
            if (move.WasPromoted)
            {
                var piece = board.GetPiece(move.To);
                if (piece != null && !piece.IsKing)
                {
                    // Ta bort RegularPiece först innan vi placerar KingPiece
                    board.RemovePiece(move.To);
                    var kingPiece = new KingPiece(piece.Color, move.To);
                    board.PlacePiece(kingPiece, move.To);
                }
            }
        }
    }
}
