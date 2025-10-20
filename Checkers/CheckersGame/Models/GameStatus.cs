using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    // Enumdefinition för GameStatus
    public enum GameStatus
    {
        WaitingToStart,
        InProgress,
        Completed,
        Abandoned
    }
}
