using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Models
{
    /// <summary>
    /// definitionen för hur en vanlig pjäs rör sig
    /// </summary>
    public class RegularPiece: Piece
    {
        public RegularPiece(PieceColor color, Position position) : base(color, position)
        {
        }

        /// <summary>
        /// Klonar den aktiva pjäsens färg och position
        /// </summary>
        /// <param name="Color">Color</param>
        /// <param name="Position">Position</param>
        public override Piece Clone()
        {
            var clone = new RegularPiece(Color, Position);

            return clone;
        }

        /// <summary>
        /// Lägger giltiga drag i en lista
        /// </summary>
        /// <returns> Retunerar listan med giltiga drag </returns>
        /// <param name="board">IBoard</param>
        public override List<Position> GetValidMoves(IBoard board)
        {
            var validMoves = new List<Position>();
            validMoves.AddRange(GetForwardMoves(board));
            validMoves.AddRange(GetCaptureMoves(board));
            return validMoves;
        }


        /// <summary>
        /// Beräknar framtida position för giltiga drag, lägger det utförda draget i en lista
        /// </summary>
        /// <returns> Retunerar listan med utfört drag </returns>
        /// <param name="board">IBoard</param>
        private List<Position> GetForwardMoves(IBoard board)
        {
            var forwardMoves = new List<Position>();

            // Svarta pjäser rör sig NER (rad ökar), Röda pjäser rör sig UPP (rad minskar)
            int direction = (Color == PieceColor.Dark) ? 1 : -1;

            // Beräkna de två diagonala fram-dragen
            Position forwardLeft = new Position(Position.Row + direction, Position.Column - 1);
            Position forwardRight = new Position(Position.Row + direction, Position.Column + 1);

            if (forwardLeft.IsValid(board.Size))
            {
                forwardMoves.Add(forwardLeft);
            }

            if (forwardRight.IsValid(board.Size))
            {
                forwardMoves.Add(forwardRight);
            }

            return forwardMoves;
        }

        /// <summary>
        /// Beräknar framtida position för fångst-drag, lägger det utförda draget i en lista
        /// </summary>
        /// <returns> Retunerar listan med ett eller fler utförda drag </returns>
        /// <param name="board">IBoard</param>
        private List<Position> GetCaptureMoves(IBoard board)
        {
            var captureMoves = new List<Position>();

            // Svarta pjäser tar NER (rad ökar med 2), Röda pjäser tar UPP (rad minskar med 2)
            int direction = (Color == PieceColor.Dark) ? 2 : -2;

            // Beräkna de två diagonala landningspositionerna för capture (hoppa över motståndarens pjäs)
            Position captureLeft = new Position(Position.Row + direction, Position.Column - 2);
            Position captureRight = new Position(Position.Row + direction, Position.Column + 2);

            if (captureLeft.IsValid(board.Size))
            {
                captureMoves.Add(captureLeft);
            }

            if (captureRight.IsValid(board.Size))
            {
                captureMoves.Add(captureRight);
            }

            return captureMoves;
        }
    }
}