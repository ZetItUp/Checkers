using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.Models;

namespace Checkers.CheckersGame.History
{
    internal class GameHistory
    {
        private readonly Board _initialBoard;
        private readonly List<Move> _moves;

        public GameHistory(Board initialBoard)
        {
            _initialBoard = initialBoard;
            _moves = new List<Move>();
        }

        public void RecordMove(Move move)
        {
            move.MoveNumber = _moves.Count + 1;
            _moves.Add(move);;     
        }

        public Move GetLastMove()
        {
            if (_moves.Count == 0)
                return null;
            return _moves[_moves.Count - 1];
        }

        public List<Move> GetAllMoves()
        {
            return new List<Move>(_moves);
        }
        
        public int GetMoveCount()
        {
            return _moves.Count;
        }

        public bool Undo(Board board)
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

        public void ReplayToMove(Board board, int moveIndex)
        {
            if (moveIndex < 0 || moveIndex > _moves.Count)
                return;
            
            ResetBoard(board);

            for (int i = 0; i < moveIndex; i++)
            {
                ApplyMove(board, _moves[i]);    
            }
        }

        public void Clear()
        {
            _moves.Clear();    
        }

        private void ResetBoard(Board board)
        {
            for (int row = 0; row < board.Size; row++)
            {
                for (int col = 0; col < board.Size; col++)
                {
                    board.RemovePiece(new Position(row, col));    
                }    
            }

            var initalPieces = _initialBoard.GetAllPieces();
            foreach (var piece in  initalPieces){
                var clonedPiece = piece.Clone();
                board.PlacePiece(clonedPiece, clonedPiece.CurrentPosition);
            }
        }

        private void ApplyMove(Board board, Move move)
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
                    var kingPiece = new KingPiece(piece.Color, piece.Position);
                    board.PlacePiece(kingPiece, kingPiece.Position);
                }
            }
        }
    }
}
