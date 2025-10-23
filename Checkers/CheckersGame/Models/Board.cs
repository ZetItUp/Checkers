using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Models
{
    public class Board
    {
        private readonly Piece?[,] squares; // [,] = 2d array. den lagrar pjäser
        public int Size { get; private set; } // storleken på brädet
        public Board(int size ) // konstruktor skapar nytt bräde som anropar storleken på brädet
        {
            this.Size = size; // this.Size nuvarande klassobjeket
            squares = new Piece?[Size, Size]; // skapa rutnät som börjar som null (inga pjäser där) och är Size brett och Size högt
        }
        public void Initialize()
        {
            //rensa brädet
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if (squares[row, col] != null)
                        squares[row, col] = null;
                }
            }
 
            //räkna hur många pjäser baserat på storleken
            int piecesRows = (Size - 2) / 2; 
            if (piecesRows < 1) piecesRows = 1; //minst en rad 

            // placera svarta pjäser
            for (int row = 0; row < piecesRows; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        var piece = new RegularPiece(PieceColor.Black, new Position(row, col));
                        PlacePiece(piece, new Position(row, col));
                    }
                }
            }

            //placera röda pjäser
            int redStartRow = Size - piecesRows;
            for (int row = redStartRow; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        var piece = new RegularPiece(PieceColor.Red, new Position(row, col));
                        PlacePiece(piece, new Position(row, col));
                    }
                }
            }
        }
        public Piece? GetPiece(Position position) // hämtar pjäsen från en viss ruta, returnerar null om rutan är tom
        {
            // Måste göra en bounds check här
            if (position.IsValid(Size))
            {
                return squares[position.Row, position.Column];
            }
            else
            {
                return null;
            }
        }

        public void PlacePiece(Piece piece, Position position)
        {
            if (piece == null)
                throw new ArgumentNullException(nameof(piece), "Piece cannot be null.");

           
            int r = position.Row;
            int c = position.Column;

            // check board bounds
            if (r < 0 || r >= Size || c < 0 || c >= Size)
                throw new ArgumentOutOfRangeException(
                    nameof(position),
                    $"Position ({r}, {c}) is out of board bounds {Size}x{Size}.");

            // check if square is empty
            if (squares[r, c] != null)
                throw new InvalidOperationException($"Square at position ({r}, {c}) is already occupied.");

            // place the piece
            squares[r, c] = piece;
        }


        public void MovePiece(Position from, Position to)
        {
            int fr = from.Row, fc = from.Column;
            int tr = to.Row, tc = to.Column;

            if (fr < 0 || fr >= Size || fc < 0 || fc >= Size)
                throw new ArgumentOutOfRangeException(nameof(from),
                    $"Source position ({fr}, {fc}) is out of board bounds {Size}x{Size}.");

            if  
              (tr < 0 || tr >= Size || tc < 0 || tc >= Size)
                throw new ArgumentOutOfRangeException(nameof(to),
                    $"Target position ({tr}, {tc}) is out of board bounds {Size}x{Size}.");
            
            if (fr == tr && fc == tc)
                throw new InvalidOperationException("Source and destination positions are the same.");

           
            var piece = squares[fr, fc];
            if (piece == null)
                throw new InvalidOperationException($"No piece at ({fr},{fc}) to move.");

            
            if (squares[tr, tc] != null)
                throw new InvalidOperationException($"Destination ({tr},{tc}) is already occupied.");

            
            squares[fr, fc] = null;
            squares[tr, tc] = piece;
            piece.Position = to;
        }

        public void RemovePiece(Position position)
        {

            int r = position.Row;
            int c = position.Column;

            
            if (r < 0 || r >= Size || c < 0 || c >= Size)
                throw new ArgumentOutOfRangeException(nameof(position),
                    $"Position ({r},{c}) is outside the board {Size}x{Size}.");

            
            if (squares[r, c] == null)
                throw new InvalidOperationException(
                    $"No piece found at position ({r},{c}) to remove.");

            
            squares[r, c] = null;
        }
        public List<Piece> GetAllPieces() // returnernar en lista med alla pjäser på brädet (oavsett färg)
        {  
            var result = new List<Piece>();

            for (int r = 0; r < Size; r++) //loopar varje rad i brädet
            {
                for (int c = 0; c < Size; c++) //loopar varje kolumn i raden
                {
                    var p = squares[r, c]; // hämtar pjäsen eller null från rutan

                    if (p != null) // är rutan inte tom finnn en pjäs
                    {
                        result.Add(p); // lägg till pjäsen
                    }
                }
            }
            return result;
        }

        public List<Piece> GetAllPieces(PieceColor color) // Overload: returnerar alla pjäser av den valda färgen
        {
            var allPieces = GetAllPieces(); // hämtar alla färger på brädet
            var result = new List<Piece>(); // Tom lista som ska innehålla pjäser av rätt färg
            foreach (var piece in allPieces) // Loopar igenom alla pjäser
            {
                if (piece.Color == color) // Har pjäsen den färg vi söker så..
                    result.Add(piece);    // lägg till den i listan
            }
            return result;
        }
        public int CountPieces(PieceColor color)   // Räknar antalet pjäser som finns av en färg och returnerar antalet
        {
            var pieces = GetAllPieces(color); // Hämtar alla pjäser av den valda färgen
            return pieces.Count; // Returnerar antalet pjäser i listan


        }

        public Board Clone() // Skapar och returnerar en kopia av brädet
        {
            var copy = new Board(Size); //  Skapar nytt bräde av samma storlek

            for (int r = 0; r < Size; r++) //  Loopar igenom varje rad
                for (int c = 0; c < Size; c++) //  Loopar igenom varje kolumn
                {
                    var  piece = squares[r, c];
                    copy.squares[r, c] = piece?.Clone();
                }
            return copy; // Returnerar brädet i form av en kopia
        }


    }
}
// work in progress

// get piece som säger vilken piece som är på den positionen 
// board ska anropa move piece
// new piece
// piece ska kunna vara nulla
// vad är det för piece på platsen  
// PlacePiece inistialiseras när vi gör spelpllan och göra den med for loop
// MovePiece - flytta pjäs från en position från en annan
// Boarden bryr sig inte om nåt annat




// prio varje klass och funktion finns inte vad som finns i dom




