using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IRuleSetFactory _ruleSetFactory;
        private IBoard? _board;
        private Player _player1;
        private Player _player2;
        private Player _currentPlayer;
        private IMoveValidator? _moveValidator;
        private IGameHistory? _gameHistory;
        private PieceOperationsService? _pieceOperationsService;
        private GameStatus _gameStatus;
        private bool _isInMultiJump = false; // Tracker om vi är i en multi-jump sekvens

        /// <summary>
        /// Indikerar om spelet är i en multi-jump sekvens
        /// </summary>
        public bool IsInMultiJump => _isInMultiJump;

        public IRuleSet? RuleSet { get; private set; }
        
        /// <summary>
        /// Constructor med DI för tester
        /// </summary>
        /// <param name="ruleSetFactory"></param>
        public GameService(IRuleSetFactory ruleSetFactory)
        {
            _ruleSetFactory = ruleSetFactory;
            _player1 = new Player("Unknown Player1", PieceColor.Light);
            _player2 = new Player("Unknown Player2", PieceColor.Dark);
            _currentPlayer = _player1;
            _gameStatus = GameStatus.WaitingToStart;
            _isInMultiJump = false;
        }

        /// <summary>
        /// Tom konstruktor, skapar en ny RuleSetFactory
        /// </summary>
        public GameService() : this(new RuleSetFactory())
        {
        }

        /// <summary>
        /// Initialiserar ett nytt spel
        /// </summary>
        /// <param name="player1Name">Name of player 1</param>
        /// <param name="player2Name">Name of player 2</param>
        /// <returns>True if a game was successfully initialized</returns>
        public bool InitializeGame(string player1Name, string player2Name)
        {
            try
            {
                RuleSet = null;
                RuleSet = _ruleSetFactory.CreateFromJsonFile(AppDomain.CurrentDomain.BaseDirectory + "Content/RuleConfig.json");
                
                if (RuleSet == null)
                {
                    return false;
                }

                _board = new Board(RuleSet.BoardSize);
                _moveValidator = new MoveValidator(RuleSet);
                _pieceOperationsService = new PieceOperationsService(RuleSet, _moveValidator);
                _board.Initialize();

                ///init History med brädet som det var initialt
                //där efter behöver vi bara spara drag
                _gameHistory = new GameHistory(_board.Clone());

                _player1 = new Player(player1Name, PieceColor.Light);
                _player2 = new Player(player2Name, PieceColor.Dark);
                _currentPlayer = _player1;
                _gameStatus = GameStatus.WaitingToStart;
                _isInMultiJump = false;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Startar spelet
        /// </summary>
        public void StartGame()
        {
            if (_gameStatus == GameStatus.WaitingToStart)
                _gameStatus = GameStatus.InProgress;
        }

        /// <summary>
        /// Gör ett drag från en position till en annan
        /// </summary>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <returns>True on successfull move</returns>
        public bool MakeMove(Position from, Position to)
        {
            if(_gameStatus != GameStatus.InProgress)
                return false;

            if(_gameHistory == null || RuleSet == null || _pieceOperationsService == null || _moveValidator == null || _board == null || _currentPlayer == null)
                return false;

            if (!_moveValidator.ValidateMove(from, to, _board, _currentPlayer))
            {
                return false;
            }

            //skapa ett move object för att spara movet
            var move = new Move(from, to);

            //kolla om en pjäs vart tagen
            var capturedPiece = _pieceOperationsService.HandleCapture(from, to, _board);
            bool wasCapture = capturedPiece != null;
            if (capturedPiece != null)
            {
                move.CapturedPiece = capturedPiece;
            }

            //flytta pjäsen på Board
            _board.MovePiece(from, to);

            //kolla om pjäsen ska bli en Dam (king)
            var piece = _board.GetPiece(to);
            if (piece != null && !piece.IsKing && _pieceOperationsService != null && _pieceOperationsService.IsPromotionPosition(to, piece.Color))
            {
                _pieceOperationsService.PromoteToKing(to, piece, _board);
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

            // Kolla om pjäsen kan ta igen 
            // Bara om: det var ett capture och AllowMultipleJumps är på
            if (wasCapture && RuleSet.AllowMultipleJumps)
            {
                if (CanPieceCaptureAgain(to))
                {
                    // Pjäsen kan ta igen 
                    _isInMultiJump = true;
                    return true;
                }
            }

            
            _isInMultiJump = false;
            SwitchTurn();

            return true;
        }

        /// <summary>
        /// Ångra senast registrerade drag
        /// </summary>
        /// <returns>True on success</returns>
        public bool Undo()
        {
            if(_gameHistory == null || _board == null || _gameStatus != GameStatus.InProgress)
                return false;

            bool success = _gameHistory.Undo(_board);

            // Byt tillbaka tur om undo lyckades
            if (success)
            {
                SwitchTurn();
            }

            return success;
        }

        /// <summary>
        /// Koll om det finns en vinnare
        /// </summary>
        /// <returns>Player if any winner, else null</returns>
        public Player? CheckWinner()
        {
            if(_board == null || _moveValidator == null)
                return null;

            //om spelaren inte har nå pjäserkvar så förlorar dom
            if (_board.CountPieces(_player1.Color) == 0)
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

        /// <summary>
        /// Ändra tur till nästa spelare
        /// </summary>
        public void SwitchTurn()
        {
            _currentPlayer = _currentPlayer.Color == _player1.Color ? _player2 : _player1;
        }

        /// <summary>
        /// Hämta ett bräde
        /// </summary>
        /// <returns>IBoard?</returns>
        public IBoard? GetBoard()
        {
            return _board;
        }

        /// <summary>
        /// Returnerar den nuvarande spelaren
        /// </summary>
        /// <returns>Player object, null if no current player</returns>
        public Player? GetCurrentPlayer()
        {
            return _currentPlayer;    
        }

        /// <summary>
        /// Hämta spelets status
        /// </summary>
        /// <returns>GameStatus</returns>
        public GameStatus GetGameStatus()
        {
            return _gameStatus;
        }

        /// <summary>
        /// Hämta spelets historik
        /// </summary>
        /// <returns>IGameHistory?</returns>
        public IGameHistory? GetGameHistory()
        {
            return _gameHistory;
        }

        /// <summary>
        /// Hämta namn på spelare 1
        /// </summary>
        /// <returns>Player name of first player</returns>
        public string GetPlayer1Name()
        {
            return _player1?.Name ?? "Player 1";
        }

        /// <summary>
        /// Hämta namn på spelare 2
        /// </summary>
        /// <returns>Player name of second player</returns>
        public string GetPlayer2Name()
        {
            return _player2?.Name ?? "Player 2";
        }

        /// <summary>
        /// Hämta alla nuvarande giltiga drag för en pjäs på en given position
        /// </summary>
        /// <param name="position">Piece current position</param>
        /// <returns>List of possible positions</returns>
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

            // Kolla att brädet och nuvarande spelare är initierade
            if (_board == null || _currentPlayer == null)
                return validMoves;

            // Hämta alla möjliga drag från pjäsen
            var potentialMoves = piece.GetValidMoves(_board);

            // Filtrera bara de drag som är faktiskt giltiga enligt MoveValidator
            foreach (var move in potentialMoves)
            {
                if (_moveValidator != null && _moveValidator.ValidateMove(position, move, _board, _currentPlayer))
                {
                    // Om vi är i multi-jump läge, tillåt BARA capture-moves
                    if (_isInMultiJump)
                    {
                        if (_moveValidator.IsCapture(position, move))
                        {
                            validMoves.Add(move);
                        }
                    }
                    else
                    {
                        validMoves.Add(move);
                    }
                }
            }

            return validMoves;
        }

        /// <summary>
        /// Gör en kontroll om en pjäs kan ta igen efter ett capture-drag
        /// </summary>
        /// <param name="piecePosition">Current piece position</param>
        /// <returns>True if piece can capture again</returns>
        private bool CanPieceCaptureAgain(Position piecePosition)
        {
            if(_board == null)
            {
                return false;
            }

            var piece = _board.GetPiece(piecePosition);

            if ( _moveValidator == null || piece == null || piece.Color != _currentPlayer.Color)
                return false;

            var validMoves = piece.GetValidMoves(_board);

            foreach (var move in validMoves)
            {
                // Kolla om draget är ett capture
                if (!_moveValidator.IsCapture(piecePosition, move))
                    continue;

                // Kolla om destinationen är tom
                if (_board.GetPiece(move) != null)
                    continue;

                // Kolla om det finns en motståndarpjäs att ta
                var capturedPos = _moveValidator.GetCapturedPosition(piecePosition, move);
                if (capturedPos.HasValue)
                {
                    var capturedPiece = _board.GetPiece(capturedPos.Value);
                    if (capturedPiece != null && capturedPiece.Color != _currentPlayer.Color)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Avslutar nuvarande spelares tur
        /// </summary>
        public void EndTurn()
        {
            if (RuleSet == null || _gameStatus != GameStatus.InProgress || RuleSet.ForcedCaptures)
                return;

            _isInMultiJump = false;
            SwitchTurn();
        }
    }
}
