using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    // Klassdefinitionen för Player
    public class Player
    {
        // Egenskap för Namn
        public string Name { get; init; }

        // Egenskap för Färg
        public PieceColor Color { get; init; }

        // Konstruktor som tar emot namn och färg och initierar egenskaperna med dessa värden.
        public Player(string name, PieceColor color)
        {
            // Tilldelar de inkommande värdena till klassens egenskaper
            Name = name;
            Color = color;
        }
    }
}
