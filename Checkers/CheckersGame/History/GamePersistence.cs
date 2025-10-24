using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.GameService;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.Validation;

namespace Checkers.CheckersGame.History;

public static class GamePersistence
{
    private static readonly string SaveDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

    static GamePersistence()
    {
        Directory.CreateDirectory(SaveDirectory);
    }

    /// Sparar ett spel automatiskt med timestamp som filnamn
    public static string SaveGame(GameService.GameService game)
    {
        string fileName = $"game_{DateTime.Now:yyyy-MM-dd_HHmmss_fff}";
        var gameState = SerializeGame(game);
        string json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
        string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
        File.WriteAllText(filePath, json);
        return fileName;
    }

    /// Laddar ett sparat spel för replay
    public static SavedGame? LoadGameForReplay(string fileName)
    {
        try
        {
            string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
            if (!File.Exists(filePath))
                return null;

            string json = File.ReadAllText(filePath);
            var gameState = JsonSerializer.Deserialize<GameSaveState>(json);

            if (gameState == null)
                return null;

            return DeserializeGame(gameState, fileName);
        }
        catch
        {
            return null;
        }
    }

    /// Hämtar alla sparade spel med metadata för UI-listan
    public static List<GameMetadata> GetSavedGamesWithMetadata()
    {
        var files = Directory.GetFiles(SaveDirectory, "*.json");
        var gameMetadataList = new List<GameMetadata>();

        foreach (var filePath in files)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var gameState = JsonSerializer.Deserialize<GameSaveState>(json);

                if (gameState != null)
                {
                    gameMetadataList.Add(new GameMetadata
                    {
                        FileName = Path.GetFileNameWithoutExtension(filePath),
                        Player1Name = gameState.Player1Name,
                        Player2Name = gameState.Player2Name,
                        DatePlayed = gameState.DatePlayed,
                        Winner = gameState.Winner,
                        TotalMoves = gameState.Moves.Count,
                        BoardSize = gameState.RuleSet.BoardSize
                    });
                }
            }
            catch
            {
                // Skippa filer som inte kan läsas
                continue;
            }
        }

        // Sortera efter datum, senaste först
        return gameMetadataList.OrderByDescending(g => g.DatePlayed).ToList();
    }

    /// Hämtar filnamn för alla sparade spel (för bakåtkompatibilitet)
    public static List<string> GetSavedGames()
    {
        var files = Directory.GetFiles(SaveDirectory, "*.json");
        var fileNames = new List<string>();

        foreach (var file in files)
        {
            fileNames.Add(Path.GetFileNameWithoutExtension(file));
        }

        return fileNames;
    }

    public static void DeleteGame(string fileName)
    {
        string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    #region Serialization

    private static GameSaveState SerializeGame(GameService.GameService game)
    {
        var history = game.GetGameHistory();
        var moves = history.GetAllMoves();

        // Bestäm vinnare baserat på spelstatus
        string? winner = null;
        if (game.GetGameStatus() == GameStatus.Completed)
        {
            // Den som är current player vann (eftersom SwitchTurn() aldrig anropades efter vinsten)
            var currentPlayer = game.GetCurrentPlayer();
            winner = currentPlayer.Name;
        }

        return new GameSaveState
        {
            Player1Name = game.GetPlayer1Name(),
            Player2Name = game.GetPlayer2Name(),
            RuleSet = new RuleSetSaveState
            {
                Name = game.RuleSet.Name,
                BoardSize = game.RuleSet.BoardSize,
                ForcedCaptures = game.RuleSet.ForcedCaptures,
                AllowBackwardCaptures = game.RuleSet.AllowBackwardCaptures,
                AllowMultipleJumps = game.RuleSet.AllowMultipleJumps
            },
            Moves = SerializeMoves(moves),
            DatePlayed = DateTime.Now,
            Winner = winner
        };
    }

    private static List<MoveSaveState> SerializeMoves(List<Move> moves)
    {
        var result = new List<MoveSaveState>();

        foreach (var move in moves)
        {
            result.Add(new MoveSaveState
            {
                FromRow = move.From.Row,
                FromColumn = move.From.Column,
                ToRow = move.To.Row,
                ToColumn = move.To.Column
            });
        }

        return result;
    }

    #endregion

    #region Deserialization

    private static SavedGame DeserializeGame(GameSaveState gameState, string fileName)
    {
        // Rekonstruera RuleSet från sparad data
        var ruleSet = new RuleSet(
            name: gameState.RuleSet.Name,
            boardSize: gameState.RuleSet.BoardSize,
            forcedCaptures: gameState.RuleSet.ForcedCaptures,
            allowBackwardCaptures: gameState.RuleSet.AllowBackwardCaptures,
            allowMultipleJumps: gameState.RuleSet.AllowMultipleJumps
        );

        // Konvertera moves från DTO till Move-objekt
        var moves = new List<Move>();
        foreach (var moveState in gameState.Moves)
        {
            var from = new Position(moveState.FromRow, moveState.FromColumn);
            var to = new Position(moveState.ToRow, moveState.ToColumn);

            // Vi behöver inte captured piece eller promotion här - de räknas ut när vi replayer
            moves.Add(new Move(from, to));
        }

        return new SavedGame
        {
            FileName = fileName,
            Player1Name = gameState.Player1Name,
            Player2Name = gameState.Player2Name,
            RuleSet = ruleSet,
            Moves = moves,
            DatePlayed = gameState.DatePlayed,
            Winner = gameState.Winner
        };
    }

    #endregion

    #region DTOs

    /// DTO för att spara speldata till JSON
    private class GameSaveState
    {
        public string Player1Name { get; set; } = "";
        public string Player2Name { get; set; } = "";
        public RuleSetSaveState RuleSet { get; set; } = new();
        public List<MoveSaveState> Moves { get; set; } = new();
        public DateTime DatePlayed { get; set; }
        public string? Winner { get; set; }
    }

    /// DTO för att spara RuleSet
    private class RuleSetSaveState
    {
        public string Name { get; set; } = "";
        public int BoardSize { get; set; }
        public bool ForcedCaptures { get; set; }
        public bool AllowBackwardCaptures { get; set; }
        public bool AllowMultipleJumps { get; set; }
    }

    /// DTO för att spara moves - endast from/to behövs, resten räknas ut vid replay
    private class MoveSaveState
    {
        public int FromRow { get; set; }
        public int FromColumn { get; set; }
        public int ToRow { get; set; }
        public int ToColumn { get; set; }
    }

    #endregion
}

/// Representerar ett laddat spel redo för replay
public class SavedGame
{
    public string FileName { get; set; } = "";
    public string Player1Name { get; set; } = "";
    public string Player2Name { get; set; } = "";
    public RuleSet RuleSet { get; set; } = null!;
    public List<Move> Moves { get; set; } = new();
    public DateTime DatePlayed { get; set; }
    public string? Winner { get; set; }
}

/// Metadata för ett sparat spel (för att visa i game list UI)
public class GameMetadata
{
    public string FileName { get; set; } = "";
    public string Player1Name { get; set; } = "";
    public string Player2Name { get; set; } = "";
    public DateTime DatePlayed { get; set; }
    public string? Winner { get; set; }
    public int TotalMoves { get; set; }
    public int BoardSize { get; set; }

    public string GetDisplayName()
    {
        return $"{Player1Name} vs {Player2Name} - {DatePlayed:yyyy-MM-dd HH:mm}";
    }

    public string GetWinnerDisplay()
    {
        return Winner ?? "Ingen vinnare";
    }
}
