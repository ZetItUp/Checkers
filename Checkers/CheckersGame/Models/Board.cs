using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.CheckersGame.Models
{
    internal class Board
    {
        List<Vector2> cordinates = new List<Vector2>();
        private readonly Piece?[,] squares; // [,] = 2d array. den lagrar pjäser
        public int Size { get; } // storleken på brädet
        public Board(int Size = 8) // konstruktor skapar nytt bräde med angedd storlek
        {

        }
        public Piece? GetPiece(Position position) // hämtar pjäsen från en viss ruta, returnerar null om rutan är tom
        {
            return null;
        }

        public List<Piece> GetAllPieces() // returnernar en lista med alla pjäser på brädet (oavsett färg)
        {  
            return null; 
        }

        public List<Piece> GetAllPieces(PieceColor color) // Overload: returnerar alla pjäser av angiven färg
        {
            return null;
        }

        public int CountPieces(PieceColor color) // Räknar hur många pjäser det finns av en viss färg och returnerar antalet
        {
            return 0;
        }

        public Board Clone() // skapar och returnerar en kopia av brädet (ny board med samma innehåll) 
        {
            return null;
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




