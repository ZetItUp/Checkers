using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.DataTypes;

namespace Checkers.CheckersGame.Validation
{
    public class MoveValidator : IMoveValidator
    {
        private readonly RuleSet _ruleSet;

        public MoveValidator(RuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        public bool ValidateMove(Position from, Position to, IBoard board, Player player)
        {
            var piece = board.GetPiece(from);

            if (piece == null)
            {
                return false;
            }

            if(!IsPieceOwnedByPlayer(piece, player))
                return false;
            if(!IsDestinationEmpty(board, to))
                return false;
            if(!IsMoveInValidList(piece, to, board))
                return false;

            // Om det är ett capture-drag, validera att det finns en motståndarens pjäs att ta
            if (IsCapture(from, to))
            {
                var capturedPosition = GetCapturedPosition(from, to);
                if (capturedPosition.HasValue)
                {
                    var capturedPiece = board.GetPiece(capturedPosition.Value);

                    // Det måste finnas en pjäs på captured-positionen
                    if (capturedPiece == null)
                        return false;

                    // Den tagna pjäsen måste vara motståndarens (motsatt färg)
                    if (capturedPiece.Color == player.Color)
                        return false;
                }
            }

            //kolla om vi måste ta (forced captures)
            if (_ruleSet.ForcedCaptures){
                // Kolla om det finns några GILTIGA captures för spelaren
                bool hasValidCapture = false;
                var playerPieces = board.GetAllPieces(player.Color);

                foreach (var p in playerPieces){
                    var possibleMoves = p.GetValidMoves(board);
                    foreach (var possibleMove in possibleMoves)
                    {
                        // Är det ett capture-drag?
                        if (IsCapture(p.Position, possibleMove))
                        {
                            // Kolla om destinationen är tom
                            if (board.GetPiece(possibleMove) != null)
                                continue;

                            // Kolla om det är ett GILTIGT capture (finns motståndarpjäs)
                            var capturedPos = GetCapturedPosition(p.Position, possibleMove);
                            if (capturedPos.HasValue)
                            {
                                var capturedPiece = board.GetPiece(capturedPos.Value);
                                // Finns det en motståndarpjäs att ta?
                                if (capturedPiece != null && capturedPiece.Color != player.Color)
                                {
                                    hasValidCapture = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (hasValidCapture) break;
                }

                // Om det finns giltiga captures, måste detta drag OCKSÅ vara ett capture
                if (hasValidCapture)
                {
                    return IsCapture(from, to);
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

        public bool HasValidMoves(Player player, IBoard board)
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

        private bool IsPieceOwnedByPlayer(Piece piece, Player player)
        {
            return piece != null && piece.Color == player.Color;
        }

        private bool IsDestinationEmpty(IBoard board, Position to)
        {
            return board.GetPiece(to) ==  null;
        }

        private bool IsMoveInValidList(Piece piece, Position to, IBoard board)
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
