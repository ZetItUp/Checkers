using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.Models;

namespace Checkers.CheckersGame.Validation
{
    internal class MoveValidator
    {
        private readonly RuleSet _ruleSet;

        public MoveValidator(RuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        public bool ValidateMove(Position from, Position to, Board board, Player player)
        {
            var piece = board.GetPiece(from);
            
            if(!IsPieceOwnedByPlayer(piece, player))
                return false;
            if(!IsDestinationEmpty(board, to))
                return false;
            if(!IsMoveInValidList(piece, to, board))
                return false;
            
            //kolla om vi måste ta
            if (_ruleSet.ForcedCaptures){
                var playerPieces = board.GetAllPieces(player.Color);

                foreach (var p in playerPieces){
                    var validMoves = p.GetValidMoves(board);
                    foreach (var validMove in validMoves)
                    {
                        if (IsCapture(p.Position, validMove))
                        {
                            return IsCapture(from, to);
                        }    
                    }
                }
            }
            return true;
        }

        public Position? GetCapturedPosition(Position from, Position to)
        {
            if(!IsCapture(from, to))
                return null;
            int rowDiff = to.Row - from.Row;
            int colDiff = to.Column - from.Column;

            int captureRow = from.Row + (rowDiff / 2);
            int captureCol = from.Column +  (colDiff / 2);
            
            return new Position(captureRow, captureCol);
        }

        public bool IsCapture(Position from, Position to)
        {
            int rowDiff = Math.Abs(to.Row - from.Row);
            int colDiff = Math.Abs(to.Column - from.Column);
            
            return rowDiff == 2 && colDiff == 2;
        }

        public bool HasValidMoves(Player player, Board board)
        {
            var playerPieces = board.GetAllPieces(player.Color);

            foreach (var piece in playerPieces)
            {
                var validMoves = piece.GetValidMoves(board);
            
                // om forcecapture är på, filtrerar för drag som kan ta en pjäs (om nån)
                if (_ruleSet.ForcedCaptures)
                {
                    bool hasCaptures = false;
                    foreach (var move in validMoves)
                    {
                        if (IsCapture(piece.Position, move))
                        {
                            hasCaptures = true;
                            break;
                        }
                    }

                    if (hasCaptures)
                    {
                        //om force capture är på, kolla om det finns några pjäser att ta
                        foreach (var move in validMoves)
                        {
                            if (IsCapture(piece.Position, move))
                            {
                                return true;    
                            }
                        }
                    }
                    else
                    {
                        //inga pjäser att ta
                        if(validMoves.Count > 0)
                            return true;
                    }
                }
                else
                {
                    //force capture är av
                    if (validMoves.Count > 0)
                        return true;    
                }
            }
            return false;
        }

        private bool IsPiceOwnedByPlayer(Piece piece, Player player)
        {
            return piece != null && piece.Color == player.Color;
        }

        private bool IsDestionationEmpty(Board board, Position to)
        {
            return board.GetPiece(to) ==  null;
        }

        private bool IsMoveInValidList(Piece piece, Position to, Board board)
        {
            var validMoves = piece.GetValidMoves(board);

            foreach (var validMove in  validMoves )
            {
                if(validMove.Equals(to))
                    return true;
            }

            return false;
        }
    }
}
