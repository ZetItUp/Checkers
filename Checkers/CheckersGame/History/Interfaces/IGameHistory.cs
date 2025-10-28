using System.Collections.Generic;
using Checkers.CheckersGame.Models;

namespace Checkers.CheckersGame.History
{
    public interface IGameHistory
    {
        
        void RecordMove(Move move);

        
        bool Undo(IBoard board);

        
        List<Move> GetAllMoves();

        
        void Clear();
    }
}