using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.History;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.Validation;

namespace Checkers.CheckersGame.GameService
{
    public class GameService
    {
        private Board? _board;
        private Player? _player1;
        private Player? _player2;
        private Player? _currentPlayer;
        private MoveValidator? _moveValidator;
        private GameHistory? _gameHistory;
        private GameStatus _gameStatus;
        //RuleSet satt till public så gui kan läsa 
        public RuleSet RuleSet{ get; private set; }
        public GameService()
        {
            RuleSet = RuleSet.CreateStandard(); // blir annorlunda när vi har factoryn
        }

        public void InitializeGame(string player1Name, string player2Name)
        {
            _board = new Board(RuleSet.BoardSize);
            _player1 = new Player(player1Name, PieceColor.Red);
            _player2 = new Player(player2Name, PieceColor.Black);
            _currentPlayer =  _player1;
            _moveValidator = new MoveValidator(RuleSet);
            _gameStatus = GameStatus.WaitingToStart;

            _board.Initialize();
            
            ///init History med brädet som det var initialt
            //där efter behöver vi bara spara drag
            _gameHistory = new GameHistory(_board.Clone());
        }

        public void StartGame()
        {
            if (_gameStatus == GameStatus.WaitingToStart)
                _gameStatus = GameStatus.InProgress;
        }

        public bool MakeMove(Position from, Position to)
        {
            if(_gameStatus != GameStatus.InProgress)
                return false;
            if(!_moveValidator.ValidateMove(from, to, _board, _currentPlayer))
                return false;

            //skapa ett move object för att spara movet
            var move = new Move(from, to);

            //kolla om en pjäs vart tagen
            var capturedPiece = HandleCapture(from, to);
            if (capturedPiece != null)
            {
                move.CapturedPiece = capturedPiece;
            }

            //flytta pjäsen på Board
            _board.MovePiece(from, to);


            //kolla om pjäsen ska bli en Dam (king)
            var piece = _board.GetPiece(to);
            if (piece != null && !piece.IsKing && IsPromotionPosition(to, piece.Color))
            {
                PromoteToKing(to, piece);
                move.WasPromoted = true;
            }
            //spara draget i history
            _gameHistory.RecordMove(move);

            //kolla om det är en vinnare
            var winner = CheckWinner();
            if (winner != null){
                _gameStatus = GameStatus.Completed;
                return true;
            }

            SwitchTurn();

            return true;
        }

        public bool Undo()
        {
            if(_gameStatus != GameStatus.InProgress)
                return false;

            bool success = _gameHistory.Undo(_board);

            // Byt tillbaka tur om undo lyckades
            if (success)
            {
                SwitchTurn();
            }

            return success;
        }

        public Player CheckWinner()
        {
            //om spelaren inte har nå pjäserkvar så förlorar dom
            if(_board.CountPieces(_player1.Color) == 0)
               return _player2;

            if(_board.CountPieces(_player2.Color) == 0)
               return _player1;

            //om en spelare inte har nå giltiga drag kvar så förlorar dom
            // Kolla båda spelarna för giltiga drag
            bool player1HasMoves = _moveValidator.HasValidMoves(_player1, _board);
            bool player2HasMoves = _moveValidator.HasValidMoves(_player2, _board);

            if (!player1HasMoves)
                return _player2;

            if (!player2HasMoves)
                return _player1;

            return null; //ingen vinnare än
        }

        public void SwitchTurn()
        {
            _currentPlayer = _currentPlayer.Color == _player1.Color ? _player2 : _player1;
        }

        public Board GetBoard()
        {
            return _board;
        }

        public Player GetCurrentPlayer()
        {
            return _currentPlayer;    
        }

        public GameStatus GetGameStatus()
        {
            return _gameStatus;
        }

        public GameHistory GetGameHistory()
        {
            return _gameHistory;
        }

        public string GetPlayer1Name()
        {
            return _player1?.Name ?? "Player 1";
        }

        public string GetPlayer2Name()
        {
            return _player2?.Name ?? "Player 2";
        }

        public List<Position> GetValidMovesForPiece(Position position)
        {
            var validMoves = new List<Position>();

            // Kolla om spelet är i gång
            if (_gameStatus != GameStatus.InProgress)
                return validMoves;

            // Hämta pjäsen på positionen
            var piece = _board?.GetPiece(position);
            if (piece == null)
                return validMoves;

            // Kolla att pjäsen tillhör nuvarande spelaren
            if (piece.Color != _currentPlayer?.Color)
                return validMoves;

            // Hämta alla möjliga drag från pjäsen
            var potentialMoves = piece.GetValidMoves(_board);

            // Filtrera bara de drag som är faktiskt giltiga enligt MoveValidator
            foreach (var move in potentialMoves)
            {
                if (_moveValidator != null && _moveValidator.ValidateMove(position, move, _board, _currentPlayer))
                {
                    validMoves.Add(move);
                }
            }

            return validMoves;
        }

        private void PromoteToKing(Position position, Piece piece)
        {
            _board.RemovePiece(position);
            var kingPiece = new KingPiece(piece.Color, position);
            _board.PlacePiece(kingPiece, position);
        }
        private Piece HandleCapture(Position from, Position to)
        {
            var capturedPosition = _moveValidator.GetCapturedPosition(from, to);
            if (capturedPosition.HasValue){
                var capturedPiece = _board.GetPiece(capturedPosition.Value);
                // Ta bara bort pjäsen om det faktiskt finns en där
                if (capturedPiece != null)
                {
                    _board.RemovePiece(capturedPosition.Value);
                }
                return capturedPiece;
            }
            return null;
        }

        private bool IsPromotionPosition(Position position, PieceColor color)
        {
            if (color == PieceColor.Red && position.Row == 0)
                return true;
            
            if(color == PieceColor.Black && position.Row == RuleSet.BoardSize -1)
                return true;
            return false;
        }
    }
}
