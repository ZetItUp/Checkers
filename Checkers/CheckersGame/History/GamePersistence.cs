using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.GameService;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.Validation;
using Microsoft.Xna.Framework;

namespace Checkers.CheckersGame.History;

public static class GamePersistence
{
    private static readonly string SaveDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Checkers",
        "Saves");

    static GamePersistence()
    {
        Directory.CreateDirectory(SaveDirectory);
    }

    public static void SaveGame(GameService.GameService game, string fileName)
    {
        string json = SerializeGame(game);
        string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
        File.WriteAllText(filePath, json);
    }

    // public static GameService.GameService LoadGame(string fileName)
    // {
    //     try{
    //         string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
    //         if (!File.Exists(filePath))
    //             return null;
    //
    //         string json = File.ReadAllText(filePath);
    //         //return DeserializeGame(json); ta bort komentaren när DeserializeGame är fixadi
    //         
    //     }
    //     catch
    //     {
    //         return null;    
    //     }
    // }

    public static List<string> GetSavedGames()
    {
        var files = Directory.GetFiles(SaveDirectory, "*.json");
        var fileNames = new List<string>();

        foreach (var file in  files){
            fileNames.Add(Path.GetFileNameWithoutExtension(file));
        }
        
        return fileNames;
    }

    public static void DeleteGame(string fileName)
    {
        string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
        if(File.Exists(filePath))
            File.Delete(filePath);
    }
    

    private static string SerializeGame(GameService.GameService game)
    {
        //skapa en simple serializeable game state 
        var gameState = new GameSaveState{
            Player1Name = game.GetCurrentPlayer().Color == PieceColor.Red
                ? game.GetCurrentPlayer().Name
                : GetOpponentName(game),
            Player2Name = game.GetCurrentPlayer().Color == PieceColor.Black
                ? game.GetCurrentPlayer().Name
                : GetOpponentName(game),
            BoardSize = game.RuleSet.BoardSize,
            Status = game.GetGameStatus(),
            CurrentPlayerColor = game.GetCurrentPlayer().Color,
            // sparar bara initialt bräde + alla moves
            InitialBoardState = SerializeInitialBoard(game.GetGameHistory()),
            Moves = SerializeMoves(game.GetGameHistory().GetAllMoves())
        };
        
        return JsonSerializer.Serialize(gameState);
    }

    private static string GetOpponentName(GameService.GameService game)
    {
        return game.GetCurrentPlayer().Color == PieceColor.Red ? "Player2" : "Player1";    
    }

    private static List<PieceSaveState> SerializeInitialBoard(GameHistory gameHistory){
        var tempBoard = new Board(8);
        
        var pieces = new List<PieceSaveState>();
        var allPieces = tempBoard.GetAllPieces();

        foreach (var piece in allPieces){
            pieces.Add(new PieceSaveState{
                Row = piece.Position.Row,
                Column = piece.Position.Column,
                Color = piece.Color,
                IsKing = piece.IsKing
            });
        }
        return pieces;
    }

    private static List<MoveSaveState> SerializeMoves(List<Move> moves)
    {
        var result = new List<MoveSaveState>();

        foreach (var move in moves )
        {
            result.Add(new MoveSaveState
            {
                FromRow = move.From.Row,
                FromColumn = move.From.Column,
                ToRow = move.To.Row,
                ToColumn = move.To.Column,
                WasPromoted = move.WasPromoted,
                CapturedRow = move.CapturedPiece != null ? move.CapturedPiece.Position.Row : -1,
                CapturedColumn = move.CapturedPiece != null ? move.CapturedPiece.Position.Column : -1,
                CapturedColor = move.CapturedPiece != null ? move.CapturedPiece.Color : PieceColor.Red,
                MoveNumber = move.MoveNumber
            });   
        }
        return result;
    }

    // private static GameService.GameService DeserializeGame(string json)
    // {
    //     var gameState = JsonSerializer.Deserialize<GameSaveState>(json);
    //     
    //     RuleSet ruleSet = new RuleSet(
    //         name: gameState.RuleSetName,
    //         boardSize: gameState.BoardSize,
    //         forcedCaptures: gameState.ForcedCaptures,
    //         allowBackwardCaptures: gameState.AllowBackwardCaptures,
    //         allowMultipleJumps: gameState.AllowMultipleJumps
    //     );
    //
    //     var game = new GameService.GameService();
    //     
    //     RuleSetType ruleSetType;
    //     switch (gameState.RuleSetName)
    //     {
    //         case "International":
    //             ruleSetType = RuleSetType.International;
    //             break;
    //         case "Relaxed":
    //             ruleSetType = RuleSetType.Relaxed;
    //             break;
    //         default:
    //             ruleSetType = RuleSetType.Standard;
    //             break;
    //     }
    //     game.SetRuleSet(ruleSetType);
    //     
    //     //rensa bordet och bygg upp det från saved state
    //     var board = game.GetBoard();
    //     for (int row = 0; row < board.Size; row++)
    //     {
    //         for (int col = 0; col < board.Size; col++)
    //         {
    //             board.RemovePiece(new Position(row, col));
    //         }
    //     }
    //     foreach (var pieceState in gameState.BoardState)
    //     {
    //         var position = new Position(pieceState.Row, pieceState.Column);
    //         Piece piece;
    //             
    //         if (pieceState.IsKing)
    //         {
    //             piece = new KingPiece(pieceState.Color, position);
    //         }
    //         else
    //         {
    //             piece = new RegularPiece(pieceState.Color, position);
    //         }
    //             
    //         board.PlacePiece(piece, position);
    //     }
    //     if (gameState.Status != GameStatus.WaitingToStart)
    //     {
    //         game.Start();
    //     
    //         // Replay all the moves to reconstruct game history and current state
    //         foreach (var moveState in gameState.Moves)
    //         {
    //             var fromPos = new Position(moveState.FromRow, moveState.FromColumn);
    //             var toPos = new Position(moveState.ToRow, moveState.ToColumn);
    //         
    //             // Make the move on the board
    //             game.MakeMove(fromPos, toPos);
    //         
    //             // If we've reached the current game state (all moves have been replayed),
    //             // make sure the right player is set as current
    //             if (moveState.MoveNumber == gameState.Moves.Count && 
    //                 game.GetCurrentPlayer().Color != gameState.CurrentPlayerColor)
    //             {
    //                 // If needed, switch turn to match saved state
    //                 game.SwitchTurn();
    //             }
    //         }
    //     }
    //     return game;
    // }
    private class GameSaveState
    {
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
        public int BoardSize { get; set; }
        public GameStatus Status { get; set; }
        public PieceColor CurrentPlayerColor { get; set; }
        public List<PieceSaveState> InitialBoardState { get; set; }
        public List<MoveSaveState> Moves { get; set; }
    }

    private class PieceSaveState
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public PieceColor Color { get; set; }
        public bool IsKing { get; set; }
    }

    private class MoveSaveState
    {
        public int FromRow { get; set; }
        public int FromColumn { get; set; }
        public int ToRow { get; set; }
        public int ToColumn { get; set; }
        public bool WasPromoted { get; set; }
        public int CapturedRow { get; set; }
        public int CapturedColumn { get; set; }
        public PieceColor CapturedColor { get; set; }
        public int MoveNumber { get; set; }
    }

}
