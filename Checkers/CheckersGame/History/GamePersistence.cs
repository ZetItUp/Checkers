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

    /// <summary>
    /// Sparar ett spel automatiskt med timestamp som filnamn
    /// </summary>
    /// <param name="game">GameService</param>
    /// <returns> Retunerar strängen med filnamnet </returns>
    public static string SaveGame(GameService.GameService game)
    {
        // Säkerställ att Save-katalogen finns
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        string fileName = $"game_{DateTime.Now:yyyy-MM-dd_HHmmss_fff}";
        var gameState = SerializeGame(game);
        string json = JsonSerializer.Serialize(gameState, new JsonSerializerOptions { WriteIndented = true });
        string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
        File.WriteAllText(filePath, json);
        return fileName;
    }

    /// <summary>
    /// Laddar ett sparat spel för replay
    /// </summary>
    /// <param name="fileName">SavedGame</param>
    /// <returns> Retunerar den deserialiserade formen av den angivna parametern + spelets status vid sparande </returns>
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

    /// <summary>
    /// Hämtar alla sparade spel med metadata för UI-listan
    /// </summary>
    /// <returns> Retunerar en lista med alla sparade spel i fallande ordning </returns>
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

    /// <summary>
    /// Hämtar filnamn för alla sparade spel för bakåtkompatibilitet (vid vissa tester)
    /// </summary>
    /// <returns> Retunerar en lista med alla sparade spel för bakåtkompabilitet (vid vissa tester) </returns>
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

    /// <summary>
    /// Tar bort en specifik spelomgång
    /// </summary>
    /// <param name="fileName">GameService</param>
    public static void DeleteGame(string fileName)
    {
        string filePath = Path.Combine(SaveDirectory, $"{fileName}.json");
        if (File.Exists(filePath))
            File.Delete(filePath);
    }

    #region Serialization

    /// <summary>
    /// Serilaiserar spelets status
    /// </summary>
    /// <param name="game">GameService</param>
    /// <returns> Retunerar spelets nuvarande tillstånd, vid vinst deklareras vinnaren </returns>
    private static GameSaveState SerializeGame(GameService.GameService game)
    {
        var history = game.GetGameHistory();
        var moves = history?.GetAllMoves();

        // Bestäm vinnare baserat på spelstatus
        string? winner = null;
        if (game.GetGameStatus() == GameStatus.Completed)
        {
            // Den som är current player vann (eftersom SwitchTurn() aldrig anropades efter vinsten)
            var currentPlayer = game.GetCurrentPlayer();
            if (currentPlayer != null)
            {
                winner = currentPlayer.Name;
            }
            else
            {
                winner = "Unnamed Player";
            }
        }

        if (moves == null)
            moves = new List<Move>();

        return new GameSaveState
        {
            Player1Name = game.GetPlayer1Name(),
            Player2Name = game.GetPlayer2Name(),
            RuleSet = new RuleSetSaveState
            {
                BoardSize = game.RuleSet!.BoardSize,
                ForcedCaptures = game.RuleSet.ForcedCaptures,
                AllowMultipleJumps = game.RuleSet.AllowMultipleJumps
            },
            Moves = SerializeMoves(moves),
            DatePlayed = DateTime.Now,
            Winner = winner
        };
    }

    /// <summary>
    /// Serilaiserar listan med utförda drag
    /// </summary>
    /// <param name="moves">List<paramref name="Move"/>></param>
    /// <returns> Retunerar den serialiserade listan </returns>
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

    /// <summary>
    /// Deserialiserar en färdig spelomgång för att sparas
    /// </summary>
    /// <param name="gameState">GameSaveState</param>
    /// <param name="fileName">GameService</param>
    /// <returns> Retunerar all relevant information om spelomgången </returns>
    private static SavedGame DeserializeGame(GameSaveState gameState, string fileName)
    {
        // Rekonstruera RuleSet från sparad data
        var ruleSet = new RuleSet(
            boardSize: gameState.RuleSet.BoardSize,
            forcedCaptures: gameState.RuleSet.ForcedCaptures,
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

    /// <summary>
    /// Visar spelarnas namn och datum för spelomgången
    /// </summary>
    /// <returns>Retunerar namnet på vinnaren, om NULL retuneras "Ingen vinnare"</returns>
    public string GetDisplayName()
    {
        return $"{Player1Name} vs {Player2Name} - {DatePlayed:yyyy-MM-dd HH:mm}";
    }

    public string GetWinnerDisplay()
    {
        return Winner ?? "Ingen vinnare";
    }
}
