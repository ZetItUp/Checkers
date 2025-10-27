using System;
using System.Collections.Generic;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.GameService;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.Validation;

namespace Checkers.CheckersGame.History;

/// Hanterar replay av sparade spel
public class ReplayService
{
    private readonly SavedGame _savedGame;
    private readonly IBoard _board;
    private readonly IGameHistory _gameHistory;
    private readonly RuleSet _ruleSet;
    private readonly PieceOperationsService _pieceOperationsService;

    private int _currentMoveIndex;  // Nuvarande position i replay
    private Player _player1;
    private Player _player2;

    public string Player1Name => _savedGame.Player1Name;
    public string Player2Name => _savedGame.Player2Name;
    public Player CurrentPlayer { get; private set; }

    public int CurrentMoveIndex => _currentMoveIndex;
    public int TotalMoves => _savedGame.Moves.Count;
    public bool IsAtStart => _currentMoveIndex == 0;
    public bool IsAtEnd => _currentMoveIndex >= TotalMoves;
    public string? Winner => _savedGame.Winner;
    public DateTime DatePlayed => _savedGame.DatePlayed;

    public ReplayService(SavedGame savedGame)
    {
        _savedGame = savedGame ?? throw new ArgumentNullException(nameof(savedGame));
        _ruleSet = savedGame.RuleSet;
        _board = new Board(_ruleSet.BoardSize);
        _board.Initialize();

        _gameHistory = new GameHistory(_board.Clone());
        _currentMoveIndex = 0;

        // Skapa spelare
        _player1 = new Player(savedGame.Player1Name, PieceColor.Red);
        _player2 = new Player(savedGame.Player2Name, PieceColor.Black);

        // Röd börjar alltid
        CurrentPlayer = _player1;

        // Initiera piece operations service
        var moveValidator = new MoveValidator(_ruleSet);
        _pieceOperationsService = new PieceOperationsService(_ruleSet, moveValidator);
    }

    /// Gå framåt ett drag i replay
    public bool StepForward()
    {
        if (IsAtEnd)
            return false;

        var move = _savedGame.Moves[_currentMoveIndex];
        ExecuteMove(move.From, move.To);
        _currentMoveIndex++;

        return true;
    }

    /// Gå bakåt ett drag i replay
    public bool StepBackward()
    {
        if (IsAtStart)
            return false;

        _gameHistory.Undo(_board);
        _currentMoveIndex--;
        SwitchPlayer();

        return true;
    }

    /// Återställ replay till början
    public void ResetToStart()
    {
        _board.Initialize();
        _gameHistory.Clear();
        _currentMoveIndex = 0;
        CurrentPlayer = _player1;
    }


    /// Hämta brädet i sitt nuvarande tillstånd
    public IBoard? GetBoard() => _board;


    private void ExecuteMove(Position from, Position to)
    {
        var move = new Move(from, to);

        // Hantera capture
        var capturedPiece = _pieceOperationsService.HandleCapture(from, to, _board);
        if (capturedPiece != null)
            move.CapturedPiece = capturedPiece;

        // Flytta pjäs
        _board.MovePiece(from, to);

        // Hantera promotion
        var piece = _board.GetPiece(to);
        if (piece != null && !piece.IsKing && _pieceOperationsService.IsPromotionPosition(to, piece.Color))
        {
            _pieceOperationsService.PromoteToKing(to, piece, _board);
            move.WasPromoted = true;
        }

        // Spara i history för undo-funktionalitet
        _gameHistory.RecordMove(move);

        // Byt spelare
        SwitchPlayer();
    }

    private void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer.Color == PieceColor.Red
            ? _player2
            : _player1;
    }
}