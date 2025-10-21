using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public struct CurrentPosition
    {
        public readonly int Row { get; }

        public readonly int Column { get; }

        public CurrentPosition(int row, int column)
        {
            this.Row = row;

            this.Column = column;
        }
    }
}