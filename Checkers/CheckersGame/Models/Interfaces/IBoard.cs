using System.Collections.Generic;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Models
{
    public interface IBoard
    {
        int Size { get; }

        void Initialize();
        Piece? GetPiece(Position position);
        void PlacePiece(Piece piece, Position position);
        void MovePiece(Position from, Position to);
        void RemovePiece(Position position);
        List<Piece> GetAllPieces();
        List<Piece> GetAllPieces(PieceColor color);
        int CountPieces(PieceColor color);
        IBoard Clone();
    }
}