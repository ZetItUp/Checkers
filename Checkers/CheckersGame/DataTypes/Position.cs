using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.DataTypes
{
    public struct Position
    {
        public readonly int Row { get; }

        public readonly int Column { get; }

        public Position(int row, int column)
        {
            Row = row;

            Column = column;
        }

        public bool IsValid(int size)
        {
            return Row >= 0 && Row < size && Column >= 0 && Column < size;
        }
    }
}