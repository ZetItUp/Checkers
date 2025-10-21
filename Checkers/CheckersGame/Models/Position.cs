using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public struct Position
    {
        public readonly int Row { get; }

        public readonly int Column { get; }

        public Position(int row, int column)
        {
            this.Row = row;

            this.Column = column;
        }
    }
}