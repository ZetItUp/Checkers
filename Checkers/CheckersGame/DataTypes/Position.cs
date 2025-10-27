using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.DataTypes
{
    /// <summary>
    /// Datatyp för Position
    /// </summary>
    public struct Position
    {
        /// <summary>
        /// Positionens Rad
        /// </summary>
        public readonly int Row { get; }

        /// <summary>
        /// Positionens Kolumn
        /// </summary>
        public readonly int Column { get; }

        public Position(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Kollar om positionen är inom ett brädes storlek
        /// </summary>
        /// <param name="size">BoardSize</param>
        /// <returns>True if position is within the BoardSize</returns>
        public bool IsValid(int size)
        {
            return Row >= 0 && Row < size && Column >= 0 && Column < size;
        }

        /// <summary>
        /// Kolla om positionen är lika som en annan position
        /// </summary>
        /// <param name="other">Other position</param>
        /// <returns>True if it's the same position</returns>
        public bool Equals(Position other)
        {
            return Row == other.Row && Column == other.Column;
        }

        /// <summary>
        /// Kollar om en position är samma som en annan position
        /// </summary>
        /// <param name="obj">Position</param>
        /// <returns>True if it's the same position</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Position otherPosition)
            {
                return Equals(otherPosition);
            }
            return false;
        }

        /// <summary>
        /// Kombinera Row och Column till HashCode
        /// </summary>
        /// <returns>HashCode</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        /// <summary>
        /// Returnera Row och Column som string (Row, Column)
        /// </summary>
        /// <returns>Returnera Row och Column som string (Row, Column)</returns>
        public override string ToString()
        {
            return $"({Row}, {Column})";
        }
    }
}