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
    /// <summary>
    /// representerar själva spelbrädet, hanterar pjäser
    /// </summary>
    public class Board : IBoard
    {
        private readonly Piece?[,] squares; // [,] = 2d array. den lagrar pjäser
        public int Size { get; private set; } // storleken på brädet
        public Board(int size ) // konstruktor skapar nytt bräde som anropar storleken på brädet
        {
            this.Size = size; // this.Size nuvarande klassobjeket
            squares = new Piece?[Size, Size]; // skapa rutnät som börjar som null (inga pjäser där) och är Size brett och Size högt
        }
        /// <summary>
        /// rensar brädet och placerar ut pjäser
        /// </summary>
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
                        var piece = new RegularPiece(PieceColor.Dark, new Position(row, col));
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
                        var piece = new RegularPiece(PieceColor.Light, new Position(row, col));
                        PlacePiece(piece, new Position(row, col));
                    }
                }
            }
        }
        /// <summary>
        /// hämtar pjäsen från en viss ruta, returnerar null om rutan är tom
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Piece? GetPiece(Position position)
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
        /// <summary>
        /// en pjäs sätts på en tom ruta i brädet
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="position"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// flyttar en pjäs till en tom ruta, uppdaterar sedan positionen
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void MovePiece(Position from, Position to)
        {
            int fr = from.Row, fc = from.Column;
            int tr = to.Row, tc = to.Column;

            if (fr < 0 || fr >= Size || fc < 0 || fc >= Size)
                throw new ArgumentOutOfRangeException(nameof(from),
                    $"Source position ({fr}, {fc}) is out of board bounds {Size}x{Size}.");

            if (tr < 0 || tr >= Size || tc < 0 || tc >= Size)
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
        /// <summary>
        /// tar bort pjäsen från angiven ruta i brädet
        /// </summary>
        /// <param name="position"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
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
        /// <summary>
        /// returnernar en lista med alla pjäser på brädet (oavsett färg)
        /// </summary>
        /// <returns></returns>
        public List<Piece> GetAllPieces() 
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
        /// <summary>
        /// Overload: returnerar alla pjäser av den valda färgen
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public List<Piece> GetAllPieces(PieceColor color) 
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
        /// <summary>
        ///  Räknar antalet pjäser som finns av en färg och returnerar antalet
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public int CountPieces(PieceColor color)   
        {
            var pieces = GetAllPieces(color); // Hämtar alla pjäser av den valda färgen
            return pieces.Count; // Returnerar antalet pjäser i listan


        }
        /// <summary>
        /// Skapar och returnerar en kopia av brädet
        /// </summary>
        /// <returns></returns>
        public IBoard Clone() 
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





