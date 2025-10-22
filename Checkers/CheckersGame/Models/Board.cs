using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    public class Board
    {
        private readonly Piece?[,] squares; // [,] = 2d array. den lagrar pjäser
        public int Size { get; } // storleken på brädet
        public Board(int Size = 8) // konstruktor skapar nytt bräde med angedd storlek
        {
            this.Size = Size; // this.Size nuvarande klassobjeket
            squares = new Piece?[Size, Size]; // skapa rutnät som börjar som null (inga pjäser där) och är Size brett och Size högt
        }
        public Piece? GetPiece(Position position) // hämtar pjäsen från en viss ruta, returnerar null om rutan är tom
        {
            return squares[position.Row, position.Column];
        }

        public void PlacePiece(Piece piece, Position position)
        {
            throw new NotImplementedException();
        }
        public void MovePiece(Position from, Position to)
        {
            throw new NotImplementedException();
        }

        public void RemovePiece(Position position)
        {

            throw new NotImplementedException();
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

        public List<Piece> GetAllPieces(PieceColor color) // Overload: returnerar alla pjäser av angiven färg
        {
            var allPieces = GetAllPieces(); // hämtar alla färger på brädet
            var result = new List<Piece>(); // Tom lista som ska innehålla pjäser av rätt färg
            foreach (var piece in allPieces) // Loopar igenom alla pjäser
            {
                if (piece.Color == color) // Om pjäsen har den färg vi söker
                    result.Add(piece);    // Lägg till den i listan
            }
            return result;
        }
        public int CountPieces(PieceColor color)       // Räknar hur många pjäser det finns av en viss färg och returnerar antalet
        {
            var pieces = GetAllPieces(color); // Hämtar alla pjäser av den angivna färgen
            return pieces.Count; // Returnerar antalet pjäser i listan


        }

        public Board Clone() // Skapar och returnerar en kopia av brädet
        {
            var copy = new Board(Size); // 1. Skapar nytt bräde med samma storlek

            for (int r = 0; r < Size; r++) // 2. Loopar igenom varje rad
                for (int c = 0; c < Size; c++) // 3. Loopar igenom varje kolumn
                    copy.squares[r, c] = squares[r, c]; // 4. Kopierar pjäsen (eller null) till samma plats

            return copy; // 5. Returnerar kopian av brädet
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




