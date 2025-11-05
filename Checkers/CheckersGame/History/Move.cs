using System;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.History;
/// <summary>
/// lagrar info om ett drag
/// </summary>
public class Move
{
    public Position From{ get; }    
    public Position To{ get; }
    public Piece? CapturedPiece { get; set; }
    public bool WasPromoted { get; set; }
    public DateTime TimeStamp { get; set; }
    public int MoveNumber { get; set; }

    public Move(Position from, Position to)
    {
        From = from;
        To = to;
        CapturedPiece = null;
        WasPromoted = false;
        TimeStamp = DateTime.Now;
        MoveNumber = 0;
    }
    /// <summary>
    /// beskriver draget 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string captureText = CapturedPiece != null ? " (capture)" : "";
        string promotionText = WasPromoted ? " (promoted to king)" : "";
        return $"{MoveNumber}: {From} -> {To}{captureText}{promotionText}";
    }
    /// <summary>
    /// returnerar draget som en sträng i brädformat (tex a4xb3)
    /// </summary>
    /// <returns></returns>
    public string GetNotation()
    {
        char fromCol = (char)('a' + From.Column);
        int fromRow = 8 - From.Row;
        
        char toCol = (char)('a' + To.Column);
        int toRow = 8 - To.Row;
        
        string moveSymbol = CapturedPiece != null ? "x" : "-";
            
        return $"{fromCol}{fromRow}{moveSymbol}{toCol}{toRow}";
    }
}