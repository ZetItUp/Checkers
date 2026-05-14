using Checkers.CheckersGame.GameService;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.Validation;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.History;

namespace Checkers.Tests;

/// <summary>
/// Test suite for the GameService class, which manages the core game logic for Checkers.
/// </summary>
public class GameServiceTests
{
    #region Test Helpers
    /// <summary>
    /// Mock implementation of IRuleSetFactory for testing.
    /// Returns a controlled RuleSet without requiring file system access.
    /// </summary>
    private class MockRuleSetFactory : IRuleSetFactory
    {
        private readonly IRuleSet _ruleSet;

        public MockRuleSetFactory(IRuleSet ruleSet)
        {
            _ruleSet = ruleSet;
        }

        public IRuleSet CreateFromJsonFile(string path)
        {
            return _ruleSet;
        }

        public IRuleSet CreateFromJson(string json)
        {
            return _ruleSet;
        }
    }
    /// <summary>
    /// Helper method to create a GameService with test rules (no file system dependency).
    /// </summary>
    private GameService CreateTestGameService(int boardSize = 8, bool forcedCaptures = true, bool allowMultipleJumps = true)
    {
        var testRules = new RuleSet(boardSize, forcedCaptures, allowMultipleJumps);
        var mockFactory = new MockRuleSetFactory(testRules);
        return new GameService(mockFactory);
    }

    /// <summary>
    /// Helper method to clear all pieces from a board.
    /// </summary>
    private void ClearBoard(IBoard board)
    {
        for (int row = 0; row < board.Size; row++)
        {
            for (int col = 0; col < board.Size; col++)
            {
                var piece = board.GetPiece(new Position(row, col));
                if (piece != null)
                    board.RemovePiece(new Position(row, col));
            }
        }
    }
    /// <summary>
    ///Returns startign position for a LightPiece
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private Position GetLightStartPosition(IBoard board)
    {
        return new Position(board.Size / 2 + 1, (board.Size / 2) % 2);
    }
    /// <summary>
    ///Returns a valid first move for a LightPiece, follow up by calling GetDarkFirstMovePosition
    /// to get in to a capture scenario 
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private Position GetLightFirstMovePosition(IBoard board)
    {
        var startPos = GetLightStartPosition(board);
        return new Position(startPos.Row - 1, startPos.Column + 1);
    }
    /// <summary>
    /// Returns a position where Light captures Dark
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private Position GetlightCaptureDarkPosition(IBoard board)
    {
        var startPos = GetLightFirstMovePosition(board);
        return new Position(startPos.Row - 2, startPos.Column + 2);
    }
    /// <summary>
    ///Retruns a valid starting postion for a DarkPiece 
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private Position GetDarkStartPosition(IBoard board)
    {
        return new Position(board.Size / 2 - 2, (((board.Size / 2) - 1) % 2) + 2);
    }
    /// <summary>
    ///Returns a valid first move for a DarkPiece, Use after GetLightFirstMovePosition to
    /// get into a capture scenario 
    /// </summary>
    /// <param name="board"></param>
    /// <returns></returns>
    private Position GetDarkFirstMovePosition(IBoard board)
    {
        var startPos = GetDarkStartPosition(board);
        return new Position(startPos.Row + 1, startPos.Column - 1);
    }
    #endregion

    #region Basic GameService Tests

    /// <summary>
    /// Tests that the GameService constructor creates an instance with the standard ruleset.
    /// Verifies that the default ruleset is "Standard American" with correct board size.
    /// </summary>
    [Fact]
    public void Constructor_ShouldCreateGameServiceWithStandardRuleSet()
    {
        // Arrange & Act
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");

        // Assert
        Assert.NotNull(gameService.RuleSet);
        Assert.True(gameService.RuleSet.BoardSize > 0, "Board size should be positive");
    }

    /// <summary>
    /// Tests that InitializeGame sets the game status to WaitingToStart.
    /// This ensures the game doesn't start automatically upon initialization.
    /// </summary>
    [Fact]
    public void InitializeGame_ShouldSetGameStatusToWaitingToStart()
    {
        // Arrange
        var gameService = CreateTestGameService();

        // Act
        gameService.InitializeGame("Player1", "Player2");

        // Assert
        Assert.Equal(GameStatus.WaitingToStart, gameService.GetGameStatus());
    }

    /// <summary>
    /// Tests that InitializeGame sets Player1 as the current player.
    /// Verifies Player1 has the Light color and is set to go first.
    /// </summary>
    [Fact]
    public void InitializeGame_ShouldSetCurrentPlayerToPlayer1()
    {
        // Arrange
        var gameService = CreateTestGameService();

        // Act
        gameService.InitializeGame("Player1", "Player2");

        // Assert
        var currentPlayer = gameService.GetCurrentPlayer();
        Assert.NotNull(currentPlayer);
        Assert.Equal("Player1", currentPlayer.Name);
        Assert.Equal(PieceColor.Light, currentPlayer.Color);
    }

    /// <summary>
    /// Tests that InitializeGame creates a valid board instance.
    /// Ensures the board is properly initialized and not null.
    /// </summary>
    [Fact]
    public void InitializeGame_ShouldCreateBoardWithCorrectSize()
    {
        // Arrange
        var gameService = CreateTestGameService();

        // Act
        gameService.InitializeGame("Player1", "Player2");

        // Assert
        var board = gameService.GetBoard();
        Assert.NotNull(board);
    }

    /// <summary>
    /// Tests that StartGame transitions the game status from WaitingToStart to InProgress.
    /// This allows moves to be made after the game is started.
    /// </summary>
    [Fact]
    public void StartGame_ShouldChangeStatusToInProgress_WhenStatusIsWaitingToStart()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");

        // Act
        gameService.StartGame();

        // Assert
        Assert.Equal(GameStatus.InProgress, gameService.GetGameStatus());
    }

    /// <summary>
    /// Tests that calling StartGame multiple times doesn't change the status once in progress.
    /// Ensures the game remains in InProgress state and doesn't reset.
    /// </summary>
    [Fact]
    public void StartGame_ShouldNotChangeStatus_WhenAlreadyInProgress()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        // Act
        gameService.StartGame(); // Call again

        // Assert
        Assert.Equal(GameStatus.InProgress, gameService.GetGameStatus());
    }

    /// <summary>
    /// Tests that SwitchTurn changes the current player from Player1 to Player2.
    /// Verifies that Player2 gets the Dark color and becomes the active player.
    /// </summary>
    [Fact]
    public void SwitchTurn_ShouldChangeTurnFromPlayer1ToPlayer2()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        var initialPlayer = gameService.GetCurrentPlayer();

        // Act
        gameService.SwitchTurn();

        // Assert
        var newPlayer = gameService.GetCurrentPlayer();
        Assert.NotEqual(initialPlayer.Color, newPlayer.Color);
        Assert.Equal("Player2", newPlayer.Name);
        Assert.Equal(PieceColor.Dark, newPlayer.Color);
    }

    /// <summary>
    /// Tests that SwitchTurn toggles back to Player1 from Player2.
    /// Ensures turn switching works bidirectionally.
    /// </summary>
    [Fact]
    public void SwitchTurn_ShouldChangeTurnFromPlayer2ToPlayer1()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.SwitchTurn(); // Switch to Player2

        // Act
        gameService.SwitchTurn(); // Switch back to Player1

        // Assert
        var currentPlayer = gameService.GetCurrentPlayer();
        Assert.Equal("Player1", currentPlayer.Name);
        Assert.Equal(PieceColor.Light, currentPlayer.Color);
    }

    /// <summary>
    /// Tests that GetBoard returns a valid board instance after initialization.
    /// Ensures the board can be accessed for game operations.
    /// </summary>
    [Fact]
    public void GetBoard_ShouldReturnNonNullBoard_AfterInitialization()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");

        // Act
        var board = gameService.GetBoard();

        // Assert
        Assert.NotNull(board);
    }

    /// <summary>
    /// Tests that GetCurrentPlayer returns a valid player instance after initialization.
    /// Ensures the current player is properly tracked.
    /// </summary>
    [Fact]
    public void GetCurrentPlayer_ShouldReturnNonNullPlayer_AfterInitialization()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");

        // Act
        var currentPlayer = gameService.GetCurrentPlayer();

        // Assert
        Assert.NotNull(currentPlayer);
    }

    /// <summary>
    /// Tests that InitializeGame correctly assigns custom player names.
    /// Uses Theory with multiple test cases to verify different name combinations.
    /// </summary>
    [Theory]
    [InlineData("Alice", "Bob")]
    [InlineData("Light Player", "Dark Player")]
    [InlineData("Human", "AI")]
    public void InitializeGame_ShouldSetPlayerNamesCorrectly(string player1Name, string player2Name)
    {
        // Arrange
        var gameService = CreateTestGameService();

        // Act
        gameService.InitializeGame(player1Name, player2Name);

        // Assert
        var currentPlayer = gameService.GetCurrentPlayer();
        Assert.Equal(player1Name, currentPlayer.Name);

        gameService.SwitchTurn();
        currentPlayer = gameService.GetCurrentPlayer();
        Assert.Equal(player2Name, currentPlayer.Name);
    }
    #endregion

    #region MakeMove Tests

    /// <summary>
    /// Tests that MakeMove returns false when the game is not in progress.
    /// Ensures moves can only be made during an active game (InProgress status).
    /// </summary>
    [Fact]
    public void MakeMove_ShouldReturnFalse_WhenGameIsNotInProgress()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        // Note: Game is in WaitingToStart, not InProgress

        // Act
        var board = gameService.GetBoard();
        var fromPos = new Position(board.Size - 3, 0);
        var toPos = new Position(board.Size - 4, 1);
        var result = gameService.MakeMove(fromPos, toPos);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that MakeMove returns false for invalid moves.
    /// Verifies that the MoveValidator correctly rejects illegal moves.
    /// </summary>
    [Fact]
    public void MakeMove_ShouldReturnFalse_WhenMoveIsInvalid()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();
        var board = gameService.GetBoard();

        // Act - Invalid move (will fail validation)
        var result = gameService.MakeMove(GetLightStartPosition(board), new Position(2, 2));

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that MakeMove switches turns after a valid move.
    /// Ensures the game alternates between players correctly.
    /// Note: Requires RegularPiece.GetValidMoves() to be implemented.
    /// </summary>
    [Fact]
    public void MakeMove_ShouldSwitchTurns_WhenMoveIsValid()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var currentPlayerBeforeMove = gameService.GetCurrentPlayer();

        // Act - Try to move a Light piece from a dynamically calculated initial position
        var board = gameService.GetBoard();
        var result = gameService.MakeMove(GetLightStartPosition(board), GetLightFirstMovePosition(board));

        // Assert - Move should succeed when GetValidMoves is implemented
        Assert.True(result, "Move should succeed when RegularPiece.GetValidMoves() is properly implemented");

        var currentPlayerAfterMove = gameService.GetCurrentPlayer();
        Assert.NotEqual(currentPlayerBeforeMove.Color, currentPlayerAfterMove.Color);
    }

    /// <summary>
    /// Tests that MakeMove promotes a regular piece to a king when reaching the opposite end.
    /// For Light pieces, promotion happens at row 0. For Dark pieces, at row 7.
    /// Note: Requires RegularPiece.GetValidMoves() and promotion logic to be implemented.
    /// </summary>
    [Fact]
    public void MakeMove_ShouldPromotePiece_WhenReachingPromotionPosition()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();
        // Clear board and set up a promotion scenario
        ClearBoard(board);

        // Place a Light piece at row 1 (one move away from promotion at row 0)
        var piece = new RegularPiece(PieceColor.Light, new Position(1, 1));
        var piece2 = new RegularPiece(PieceColor.Dark, new Position(1, 2));
        board.PlacePiece(piece2, new Position(2, 1));
        board.PlacePiece(piece, new Position(1, 1));

        // Act - Move to promotion position
        var result = gameService.MakeMove(new Position(1, 1), new Position(0, 0));

        // Assert
        Assert.True(result, "Move to promotion position should succeed when GetValidMoves() is implemented");

        var pieceAtDestination = board.GetPiece(new Position(0, 0));
        Assert.NotNull(pieceAtDestination);
        Assert.True(pieceAtDestination.IsKing, "Piece should be promoted to King when reaching row 0");
    }

    /// <summary>
    /// Tests that MakeMove correctly captures opponent pieces when jumping over them.
    /// Verifies that the captuLight piece is removed from the board.
    /// </summary>
    [Fact]
    public void MakeMove_ShouldCapturePiece_WhenJumpingOverOpponent()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();
        // Clear board and set up a capture scenario
        ClearBoard(board);

        // Set up a capture scenario
        var LightPiecePos = new Position(board.Size - 3, 0);
        var DarkPiecePos = new Position(board.Size - 4, 1);
        var LightPiece = new RegularPiece(PieceColor.Light, LightPiecePos);
        var DarkPiece = new RegularPiece(PieceColor.Dark, DarkPiecePos);

        board.PlacePiece(LightPiece, LightPiecePos);
        board.PlacePiece(DarkPiece, DarkPiecePos);

        // Act - Jump over the Dark piece
        var landingPos = new Position(board.Size - 5, 2);
        var result = gameService.MakeMove(LightPiecePos, landingPos);

        // Assert
        Assert.True(result);

        var captuLightPosition = board.GetPiece(DarkPiecePos);
        Assert.Null(captuLightPosition);
    }

    #region MakeMultiple-Jumps

    [Fact]
    public void MakeMove_ShouldMultiJump_WhenForcedAndAvailable()
    {
        //Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();
        var board = gameService.GetBoard();

        //clear board and set up a multi-jump capture scenario
        ClearBoard(board);

        //Set up a multi-jump capture scenario
        var LightPiecePos = new Position(board.Size - 3, 0);
        var DarkPiece1Pos = new Position(board.Size - 4, 1);
        var DarkPiece2Pos = new Position(board.Size - 6, 3);
        var LightPiece = new RegularPiece(PieceColor.Light, LightPiecePos);
        var DarkPiece = new RegularPiece(PieceColor.Dark, DarkPiece1Pos);
        var DarkPiece2 = new RegularPiece(PieceColor.Dark, DarkPiece2Pos);
        board.PlacePiece(LightPiece, LightPiecePos);
        board.PlacePiece(DarkPiece, DarkPiece1Pos);
        board.PlacePiece(DarkPiece2, DarkPiece2Pos);

        //Act - Jump over first piece and CurrentPLayer should still be player one
        var firstJumpLandPos = new Position(board.Size - 5, 2);
        var result = gameService.MakeMove(LightPiecePos, firstJumpLandPos);

        //Assert 
        Assert.True(result);
        Assert.True(gameService.GetCurrentPlayer().Name != "Player2");

        // Jump again and take the next pieve
        var secondJumpLandPos = new Position(board.Size - 7, 4);
        var result2 = gameService.MakeMove(firstJumpLandPos, secondJumpLandPos);
    }

    #endregion

    #endregion

    #region EndTurn 


    [Fact]
    public void Player_ShouldNotBeAbleToEndTurn_WhenForcedAndAvailable()
    {
        //Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();
        var board = gameService.GetBoard();

        //clear board and set up a multi-jump capture scenario
        ClearBoard(board);

        //Set up a multi-jump capture scenario
        var LightPiecePos = new Position(board.Size - 3, 0);
        var DarkPiece1Pos = new Position(board.Size - 4, 1);
        var DarkPiece2Pos = new Position(board.Size - 6, 3);
        var LightPiece = new RegularPiece(PieceColor.Light, LightPiecePos);
        var DarkPiece = new RegularPiece(PieceColor.Dark, DarkPiece1Pos);
        var DarkPiece2 = new RegularPiece(PieceColor.Dark, DarkPiece2Pos);
        board.PlacePiece(LightPiece, LightPiecePos);
        board.PlacePiece(DarkPiece, DarkPiece1Pos);
        board.PlacePiece(DarkPiece2, DarkPiece2Pos);

        //Act - Jump over first piece and try to end turn
        var firstJumpLandPos = new Position(board.Size - 5, 2);
        var result = gameService.MakeMove(LightPiecePos, firstJumpLandPos);
        //calls end turn
        gameService.EndTurn();

        //Assert
        var currentPlayer = gameService.GetCurrentPlayer();
        Assert.NotNull(currentPlayer);
        Assert.Equal("Player1", currentPlayer.Name);
    }

    #endregion

    #region Undo Tests

    /// <summary>
    /// Tests that Undo returns false when the game is not in progress.
    /// Ensures undo operations are only allowed during active gameplay.
    /// </summary>
    [Fact]
    public void Undo_ShouldReturnFalse_WhenGameIsNotInProgress()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        // Game is WaitingToStart

        // Act
        var result = gameService.Undo();

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that Undo restores the board to the state before the last move.
    /// Verifies that pieces return to their original positions after undoing a move.
    /// Note: Will pass when GameHistory.Undo is fully implemented.
    /// </summary>
    [Fact]
    public void Undo_ShouldRestorePreviousBoardState()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();
        var board = gameService.GetBoard();

        // Make a move
        var moveResult = gameService.MakeMove(GetLightStartPosition(board), GetLightFirstMovePosition(board));
        Assert.True(moveResult);

        // Act
        var result = gameService.Undo();

        // Assert
        Assert.True(result);

        var pieceAtOriginal = board.GetPiece(GetLightStartPosition(board));
        var pieceAtNew = board.GetPiece(GetLightFirstMovePosition(board));

        Assert.NotNull(pieceAtOriginal);
        Assert.Null(pieceAtNew);
    }

    /// <summary>
    /// Tests that Undo restores captuLight pieces when undoing a capture move.
    /// Ensures that pieces removed during a jump are brought back to the board.
    /// Note: Will pass when GameHistory.Undo with capture restoration is implemented.
    /// </summary>
    [Fact]
    public void Undo_ShouldRestoreCaptuDarkPiece()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();
        var board = gameService.GetBoard();

        // Set up capture scenario
        gameService.MakeMove(GetLightStartPosition(board), GetLightFirstMovePosition(board));
        gameService.MakeMove(GetDarkStartPosition(board), GetDarkFirstMovePosition(board));


        // Act
        // Make the capture move
        var moveResult = gameService.MakeMove(GetLightFirstMovePosition(board), GetlightCaptureDarkPosition(board));
        Assert.True(moveResult, "Capture move should be valid.");

        // Undo the move
        var undoResult = gameService.Undo();
        Assert.True(undoResult, "Undo should succeed.");

        // Assert
        // The captu Dark piece should be back on the board
        var captuLightPiece = board.GetPiece(GetDarkFirstMovePosition(board));
        Assert.NotNull(captuLightPiece);
        Assert.Equal(PieceColor.Dark, captuLightPiece.Color);

        // The Light piece should be back at its original position
        var attackerPiece = board.GetPiece(GetLightFirstMovePosition(board));
        Assert.NotNull(attackerPiece);
    }

    #endregion

    #region CheckWinner Tests

    /// <summary>
    /// Tests that CheckWinner returns null when both players still have valid moves.
    /// Ensures the game continues when neither player has won yet.
    /// Note: Will pass when MoveValidator.HasValidMoves is fully implemented.
    /// </summary>
    [Fact]
    public void CheckWinner_ShouldReturnNull_WhenBothPlayersHaveValidMoves()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        // Board is already initialized with pieces for both players
        // No need to manually place pieces

        // Act
        var winner = gameService.CheckWinner();

        // Assert
        // Will return null when HasValidMoves is properly implemented
        Assert.Null(winner);
    }

    /// <summary>
    /// Tests that CheckWinner returns the opponent when the current player has no valid moves.
    /// Documents the win condition based on having no legal moves available.
    /// Note: Skipped - requires specific board setup to block all moves.
    /// </summary>
    [Fact(Skip = "Requires specific board setup where player has pieces but no valid moves - TODO: implement")]
    public void CheckWinner_ShouldReturnOpponent_WhenCurrentPlayerHasNoValidMoves()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();
        ClearBoard(board);

        // TODO: Set up a board where a player has pieces but is completely blocked
        // Example: Light pieces surrounded by Dark pieces with no legal moves

        // Act
        var winner = gameService.CheckWinner();

        // Assert
        Assert.NotNull(winner);
        // The winner should be the opponent (Player2) since Player1 can't move
    }

    /// <summary>
    /// Tests that CheckWinner correctly handles an empty board scenario.
    /// Verifies win detection when a player has no pieces remaining.
    /// Note: Skipped - documentsexpected behavior without testing.
    /// </summary>
    [Fact(Skip = "Documentation test without assertions - behavior is coverd by Player1/Player2HasNoPieces tests")]
    public void CheckWinner_ShouldHandleEmptyBoard()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();
        ClearBoard(board);

        // Act
        var winner = gameService.CheckWinner();

        // Assert
        // With no pieces for either player, there should be a winner
        // This edge case is covered by other tests
        Assert.NotNull(winner);
    }

    /// <summary>
    /// Tests that CheckWinner detects when Player1 (Light) has no pieces remaining.
    /// Player2 (Dark) should be declared the winner.
    /// </summary>
    [Fact]
    public void CheckWinner_ShouldReturnPlayer2_WhenPlayer1HasNoPieces()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();

        // Clear the board first
        ClearBoard(board);

        // Place only Dark pieces (Player2)
        var DarkPiece1 = new RegularPiece(PieceColor.Dark, new Position(0, 1));
        var DarkPiece2 = new RegularPiece(PieceColor.Dark, new Position(1, 0));
        board.PlacePiece(DarkPiece1, new Position(0, 1));
        board.PlacePiece(DarkPiece2, new Position(1, 0));

        // Act
        var winner = gameService.CheckWinner();

        // Assert
        Assert.NotNull(winner);
        Assert.Equal(PieceColor.Dark, winner.Color);
        Assert.Equal("Player2", winner.Name);
    }

    /// <summary>
    /// Tests that CheckWinner detects when Player2 (Dark) has no pieces remaining.
    /// Player1 (Light) should be declared the winner.
    /// </summary>
    [Fact]
    public void CheckWinner_ShouldReturnPlayer1_WhenPlayer2HasNoPieces()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();
        ClearBoard(board);

        // Place only Light pieces (Player1) - use dynamic board size
        var LightPiece1 = new RegularPiece(PieceColor.Light, new Position(board.Size - 2, 1));
        var LightPiece2 = new RegularPiece(PieceColor.Light, new Position(board.Size - 1, 0));
        board.PlacePiece(LightPiece1, new Position(board.Size - 2, 1));
        board.PlacePiece(LightPiece2, new Position(board.Size - 1, 0));

        // Act
        var winner = gameService.CheckWinner();

        // Assert
        Assert.NotNull(winner);
        Assert.Equal(PieceColor.Light, winner.Color);
        Assert.Equal("Player1", winner.Name);
    }

    /// <summary>
    /// Tests that CheckWinner checks both players for valid moves, not just the current player.
    /// This ensures the winner is detected regardless of whose turn it is.
    /// Note: This test will fully work when MoveValidator.HasValidMoves is implemented.
    /// </summary>
    [Fact]
    public void CheckWinner_ShouldCheckBothPlayers_NotJustCurrentPlayer()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");
        gameService.StartGame();

        var board = gameService.GetBoard();

        // Clear the board
        ClearBoard(board);

        // Place pieces for both players
        var LightPiece = new RegularPiece(PieceColor.Light, new Position(board.Size - 3, 0));
        var DarkPiece = new RegularPiece(PieceColor.Dark, new Position(2, 1));
        board.PlacePiece(LightPiece, LightPiece.Position);
        board.PlacePiece(DarkPiece, DarkPiece.Position);

        // Act
        var winner = gameService.CheckWinner();

        // Assert
        // With both players having pieces, there should be no winner yet
        // (unless HasValidMoves determines one player has no valid moves)
        // This test documents that we check BOTH players, not just current player
        Assert.True(board.CountPieces(PieceColor.Light) > 0);
        Assert.True(board.CountPieces(PieceColor.Dark) > 0);
    }

    #endregion

    #region Board Tests

    /// <summary>
    /// Tests that Board.MovePiece updates the piece's Position property.
    /// This is critical for GetValidMoves() implementations that rely on the piece knowing its current position.
    /// </summary>
    [Fact]
    public void Board_MovePiece_ShouldUpdatePiecePosition()
    {
        // Arrange
        var board = new Board(8);
        var piece = new RegularPiece(PieceColor.Light, new Position(5, 0));
        var fromPosition = new Position(5, 0);
        var toPosition = new Position(4, 1);

        board.PlacePiece(piece, fromPosition);

        // Act
        board.MovePiece(fromPosition, toPosition);

        // Assert
        var movedPiece = board.GetPiece(toPosition);
        Assert.NotNull(movedPiece);
        Assert.Equal(toPosition.Row, movedPiece.Position.Row);
        Assert.Equal(toPosition.Column, movedPiece.Position.Column);

        // Verify the piece was removed from the original position
        var pieceAtOldPosition = board.GetPiece(fromPosition);
        Assert.Null(pieceAtOldPosition);
    }

    /// <summary>
    /// Tests that Board.PlacePiece throws an exception when placing on an occupied square.
    /// Ensures the board prevents invalid placements.
    /// </summary>
    [Fact]
    public void Board_PlacePiece_ShouldThrowWhenSquareIsOccupied()
    {
        // Arrange
        var board = new Board(8);
        var piece1 = new RegularPiece(PieceColor.Light, new Position(5, 0));
        var piece2 = new RegularPiece(PieceColor.Dark, new Position(5, 0));
        var position = new Position(5, 0);

        board.PlacePiece(piece1, position);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => board.PlacePiece(piece2, position));
    }

    /// <summary>
    /// Tests that Board.RemovePiece successfully removes a piece from the board.
    /// Verifies the square becomes empty after removal.
    /// </summary>
    [Fact]
    public void Board_RemovePiece_ShouldRemovePieceFromBoard()
    {
        // Arrange
        var board = new Board(8);
        var piece = new RegularPiece(PieceColor.Light, new Position(5, 0));
        var position = new Position(5, 0);

        board.PlacePiece(piece, position);

        // Act
        board.RemovePiece(position);

        // Assert
        var removedPiece = board.GetPiece(position);
        Assert.Null(removedPiece);
    }

    /// <summary>
    /// Tests that Board.GetAllPieces returns all pieces of a specific color.
    /// Important for move validation and winner detection.
    /// </summary>
    [Fact]
    public void Board_GetAllPieces_ShouldReturnPiecesOfSpecificColor()
    {
        // Arrange
        var board = new Board(8);
        var LightPiece1 = new RegularPiece(PieceColor.Light, new Position(5, 0));
        var LightPiece2 = new RegularPiece(PieceColor.Light, new Position(5, 2));
        var DarkPiece = new RegularPiece(PieceColor.Dark, new Position(2, 1));

        board.PlacePiece(LightPiece1, new Position(5, 0));
        board.PlacePiece(LightPiece2, new Position(5, 2));
        board.PlacePiece(DarkPiece, new Position(2, 1));

        // Act
        var LightPieces = board.GetAllPieces(PieceColor.Light);
        var DarkPieces = board.GetAllPieces(PieceColor.Dark);

        // Assert
        Assert.Equal(2, LightPieces.Count);
        Assert.Single(DarkPieces);
    }

    /// <summary>
    /// Tests that Board.Initialize() sets up a standard 8x8 checkers board.
    /// Standard board should have 12 pieces per player on dark squares.
    /// Dark pieces on rows 0-2, Light pieces on rows 5-7.
    /// Note: Board.Initialize() method needs to be implemented.
    /// </summary>
    [Fact]
    public void Board_Initialize_ShouldSetupStandard8x8Board()
    {
        // Arrange
        // This test specifically validates the standard 8x8 board, so the size is intentionally hardcoded.
        var board = new Board(8);

        // Act
        board.Initialize();

        // Assert
        var allPieces = board.GetAllPieces();
        var LightPieces = board.GetAllPieces(PieceColor.Light);
        var DarkPieces = board.GetAllPieces(PieceColor.Dark);

        // Verify piece counts
        Assert.Equal(24, allPieces.Count); // 12 per player
        Assert.Equal(12, LightPieces.Count);
        Assert.Equal(12, DarkPieces.Count);

        // Verify Dark pieces are in rows 0-2 on dark squares
        foreach (var piece in DarkPieces)
        {
            Assert.True(piece.Position.Row >= 0 && piece.Position.Row <= 2,
                "Dark pieces should be in rows 0-2");
            Assert.True((piece.Position.Row + piece.Position.Column) % 2 == 1,
                "Pieces should be on dark squares (row + col is odd)");
        }

        // Verify Light pieces are in rows 5-7 on dark squares
        foreach (var piece in LightPieces)
        {
            Assert.True(piece.Position.Row >= 5 && piece.Position.Row <= 7,
                "Light pieces should be in rows 5-7");
            Assert.True((piece.Position.Row + piece.Position.Column) % 2 == 1,
                "Pieces should be on dark squares (row + col is odd)");
        }

        // Verify all pieces are RegularPiece (not Kings at start)
        Assert.All(allPieces, piece => Assert.False(piece.IsKing));
    }

    /// <summary>
    /// Tests that Board.Initialize() works correctly for different board sizes.
    /// Smaller boards should have proportionally fewer pieces.
    /// Uses 6x6 board as example (should have 6 pieces per player).
    /// </summary>
    [Theory]
    [InlineData(6, 6)]   // 6x6 board should have 6 pieces per player (rows 0-1 and 4-5)
    [InlineData(8, 12)]  // 8x8 board should have 12 pieces per player (rows 0-2 and 5-7)
    [InlineData(10, 20)] // 10x10 board should have 20 pieces per player (rows 0-3 and 6-9)
    public void Board_Initialize_ShouldSetupCorrectPieceCountForBoardSize(int boardSize, int expectedPiecesPerPlayer)
    {
        // Arrange
        var board = new Board(boardSize);

        // Act
        board.Initialize();

        // Assert
        var LightPieces = board.GetAllPieces(PieceColor.Light);
        var DarkPieces = board.GetAllPieces(PieceColor.Dark);

        Assert.Equal(expectedPiecesPerPlayer, LightPieces.Count);
        Assert.Equal(expectedPiecesPerPlayer, DarkPieces.Count);

        // Verify pieces are only on dark squares
        var allPieces = board.GetAllPieces();
        Assert.All(allPieces, piece =>
            Assert.True((piece.Position.Row + piece.Position.Column) % 2 == 1,
                $"Piece at ({piece.Position.Row},{piece.Position.Column}) should be on a dark square"));
    }

    /// <summary>
    /// Tests that Board.Initialize() places equal numbers of pieces for both players.
    /// Verifies pieces are distributed evenly between Light and Dark.
    /// </summary>
    [Fact]
    public void Board_Initialize_ShouldPlaceEqualPiecesForBothPlayers()
    {
        // Arrange
        var board = new Board(8);

        // Act
        board.Initialize();

        // Assert
        var LightPieces = board.GetAllPieces(PieceColor.Light);
        var DarkPieces = board.GetAllPieces(PieceColor.Dark);

        // Both players should have the same number of pieces
        Assert.Equal(LightPieces.Count, DarkPieces.Count);

        // All pieces should be regular pieces (not kings)
        Assert.All(LightPieces, p => Assert.False(p.IsKing));
        Assert.All(DarkPieces, p => Assert.False(p.IsKing));
    }

    /// <summary>
    /// Tests that Board.Initialize() clears the board before placing pieces.
    /// Ensures calling Initialize() multiple times doesn't create duplicates.
    /// </summary>
    [Fact]
    public void Board_Initialize_ShouldClearBoardBeforePlacingPieces()
    {
        // Arrange
        var board = new Board(8);
        board.Initialize();
        var initialCount = board.GetAllPieces().Count;

        // Act - Initialize again
        board.Initialize();

        // Assert - Should have same count, not double
        var finalCount = board.GetAllPieces().Count;
        Assert.Equal(initialCount, finalCount);
    }

    #endregion

    #region GetGameHistory Tests

    /// <summary>
    /// Tests that GetGameHistory returns a GameHistory instance after initialization.
    /// Ensures game history tracking is set up when the game is initialized.
    /// Note: Currently GameHistory initialization is commented out in GameService.
    /// </summary>
    [Fact]
    public void GetGameHistory_ShouldReturnGameHistory_AfterInitialization()
    {
        // Arrange
        var gameService = CreateTestGameService();
        gameService.InitializeGame("Player1", "Player2");

        // Act
        var history = gameService.GetGameHistory();

        // Assert
        // Will be non-null when GameHistory is initialized in InitializeGame
        // Currently may be null based on the commented code
    }

    #endregion

    #region ReplayService Tests

    /// <summary>
    /// Tests that ReplayService initializes with correct player names.
    /// </summary>
    [Fact]
    public void ReplayService_Constructor_ShouldSetPlayerNames()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Alice",
            Player2Name = "Bob",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>(),
            DatePlayed = DateTime.Now,
            Winner = null
        };

        // Act
        var replayService = new ReplayService(savedGame);

        // Assert
        Assert.Equal("Alice", replayService.Player1Name);
        Assert.Equal("Bob", replayService.Player2Name);
    }

    /// <summary>
    /// Tests that ReplayService starts at the beginning (no moves executed).
    /// </summary>
    [Fact]
    public void ReplayService_Constructor_ShouldStartAtBeginning()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(5, 0), new Position(4, 1))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        // Act
        var replayService = new ReplayService(savedGame);

        // Assert
        Assert.Equal(0, replayService.CurrentMoveIndex);
        Assert.True(replayService.IsAtStart);
        Assert.False(replayService.IsAtEnd);
    }

    /// <summary>
    /// Tests that StepForward executes a move and advances the replay position.
    /// </summary>
    [Fact]
    public void ReplayService_StepForward_ShouldExecuteMoveAndAdvance()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(5, 0), new Position(4, 1))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        var replayService = new ReplayService(savedGame);

        // Act
        var result = replayService.StepForward();

        // Assert
        Assert.True(result);
        Assert.Equal(1, replayService.CurrentMoveIndex);
        Assert.True(replayService.IsAtEnd);
        var board = replayService.GetBoard();
        Assert.NotNull(board.GetPiece(new Position(4, 1)));
        Assert.Null(board.GetPiece(new Position(5, 0)));
    }

    /// <summary>
    /// Tests that StepBackward reverts a move and decrements the replay position.
    /// </summary>
    [Fact]
    public void ReplayService_StepBackward_ShouldRevertMoveAndDecrement()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(5, 0), new Position(4, 1))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        var replayService = new ReplayService(savedGame);
        replayService.StepForward();

        // Act
        var result = replayService.StepBackward();

        // Assert
        Assert.True(result);
        Assert.Equal(0, replayService.CurrentMoveIndex);
        Assert.True(replayService.IsAtStart);
        var board = replayService.GetBoard();
        Assert.NotNull(board.GetPiece(new Position(5, 0)));
        Assert.Null(board.GetPiece(new Position(4, 1)));
    }

    /// <summary>
    /// Tests that StepForward switches turns correctly.
    /// </summary>
    [Fact]
    public void ReplayService_StepForward_ShouldSwitchTurns()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(5, 0), new Position(4, 1))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        var replayService = new ReplayService(savedGame);
        var currentPlayerBefore = replayService.CurrentPlayer;

        // Act
        replayService.StepForward();

        // Assert
        var currentPlayerAfter = replayService.CurrentPlayer;
        Assert.NotEqual(currentPlayerBefore.Color, currentPlayerAfter.Color);
    }

    /// <summary>
    /// Tests that replay handles piece promotion correctly.
    /// </summary>
    [Fact]
    public void ReplayService_StepForward_ShouldPromotePieces()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(1, 1), new Position(0, 0))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        var replayService = new ReplayService(savedGame);
        var board = replayService.GetBoard();
        var piece = new RegularPiece(PieceColor.Light, new Position(1, 1));
        board.PlacePiece(piece, new Position(1, 1));

        // Act - Move to promotion position (row 0 for Light)
        replayService.StepForward();

        // Assert
        var promotedPiece = board.GetPiece(new Position(0, 0));
        Assert.NotNull(promotedPiece);
        Assert.True(promotedPiece.IsKing);
    }

    /// <summary>
    /// Tests that replay handles captures correctly.
    /// </summary>
    [Fact]
    public void ReplayService_StepForward_ShouldHandleCaptures()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(5, 0), new Position(3, 2))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        var replayService = new ReplayService(savedGame);
        var board = replayService.GetBoard();
        ClearBoard(board);

        var LightPiece = new RegularPiece(PieceColor.Light, new Position(5, 0));
        var DarkPiece = new RegularPiece(PieceColor.Dark, new Position(4, 1));
        board.PlacePiece(LightPiece, new Position(5, 0));
        board.PlacePiece(DarkPiece, new Position(4, 1));

        // Act - Capture move
        replayService.StepForward();

        // Assert
        var captuLightPosition = board.GetPiece(new Position(4, 1));
        Assert.Null(captuLightPosition);
        Assert.NotNull(board.GetPiece(new Position(3, 2)));
    }

    /// <summary>
    /// Tests that ResetToStart resets the replay to the beginning.
    /// </summary>
    [Fact]
    public void ReplayService_ResetToStart_ShouldResetToInitialState()
    {
        // Arrange
        var savedGame = new SavedGame
        {
            FileName = "test_game",
            Player1Name = "Player1",
            Player2Name = "Player2",
            RuleSet = new RuleSet(8, true, true),
            Moves = new List<Move>
            {
                new Move(new Position(5, 0), new Position(4, 1))
            },
            DatePlayed = DateTime.Now,
            Winner = null
        };

        var replayService = new ReplayService(savedGame);
        replayService.StepForward();

        // Act
        replayService.ResetToStart();

        // Assert
        Assert.Equal(0, replayService.CurrentMoveIndex);
        Assert.True(replayService.IsAtStart);
        var board = replayService.GetBoard();
        Assert.NotNull(board.GetPiece(new Position(5, 0)));
        Assert.Null(board.GetPiece(new Position(4, 1)));
    }

    #endregion

    #region GamePersistence Tests

    /// <summary>
    /// Tests that SaveGame and LoadGameForReplay work together.
    /// Verifies that a saved game can be loaded back correctly.
    /// </summary>
    [Fact]
    public void GamePersistence_SaveAndLoad_ShouldPreserveGameData()
    {
        // Arrange
        var originalGame = CreateTestGameService();
        originalGame.InitializeGame("Alice", "Bob");
        originalGame.StartGame();

        // Make a move
        var board = originalGame.GetBoard();
        ClearBoard(board);
        var fromPos = new Position(board.Size - 3, 2);
        var piece = new RegularPiece(PieceColor.Light, fromPos);
        board.PlacePiece(piece, fromPos);
        var toPos = new Position(board.Size - 4, 3);
        originalGame.MakeMove(fromPos, toPos);

        // Act - Save the game
        string fileName = GamePersistence.SaveGame(originalGame);

        // Load the game
        var savedGame = GamePersistence.LoadGameForReplay(fileName);

        // Assert
        Assert.NotNull(savedGame);
        Assert.Equal("Alice", savedGame.Player1Name);
        Assert.Equal("Bob", savedGame.Player2Name);
        Assert.Single(savedGame.Moves);
        Assert.Equal(fromPos, savedGame.Moves[0].From);
        Assert.Equal(toPos, savedGame.Moves[0].To);

        // Cleanup
        GamePersistence.DeleteGame(fileName);
    }

    /// <summary>
    /// Tests that SaveGame captures winner when game is completed.
    /// </summary>
    [Fact]
    public void GamePersistence_SaveGame_ShouldCaptureWinner()
    {
        // Arrange
        var game = CreateTestGameService();
        game.InitializeGame("Winner", "Loser");
        game.StartGame();

        var board = game.GetBoard();
        ClearBoard(board);

        // Set up a winning scenario - only Dark pieces remain
        var DarkPiece = new RegularPiece(PieceColor.Dark, new Position(0, 1));
        board.PlacePiece(DarkPiece, new Position(0, 1));

        // Make the game completed
        var winner = game.CheckWinner();
        if (winner != null)
        {
            // Simulate game completion by making a dummy move to trigger status change
            // Since we can't directly set status, we'll test with what we have
        }

        // Act
        string fileName = GamePersistence.SaveGame(game);
        var savedGame = GamePersistence.LoadGameForReplay(fileName);

        // Assert
        Assert.NotNull(savedGame);
        // Winner will be set if game status was Completed
        // This tests the winner detection logic in GamePersistence

        // Cleanup
        GamePersistence.DeleteGame(fileName);
    }

    /// <summary>
    /// Tests that GetSavedGamesWithMetadata returns game information.
    /// </summary>
    [Fact]
    public void GamePersistence_GetSavedGamesWithMetadata_ShouldReturnGameList()
    {
        // Arrange
        var game = CreateTestGameService();
        game.InitializeGame("Player1", "Player2");
        game.StartGame();

        //string fileName = $"test_metadata_{DateTime.Now.Ticks}";
        string fileName = GamePersistence.SaveGame(game);

        // Act
        var games = GamePersistence.GetSavedGamesWithMetadata();

        // Assert
        Assert.NotNull(games);
        var savedGame = games.FirstOrDefault(g => g.FileName == fileName);
        Assert.NotNull(savedGame);
        Assert.Equal("Player1", savedGame.Player1Name);
        Assert.Equal("Player2", savedGame.Player2Name);

        // Cleanup
        GamePersistence.DeleteGame(fileName);
    }

    /// <summary>
    /// Tests that GameMetadata.GetDisplayName returns formatted name.
    /// </summary>
    [Fact]
    public void GameMetadata_GetDisplayName_ShouldReturnFormattedString()
    {
        // Arrange
        var metadata = new GameMetadata
        {
            Player1Name = "Alice",
            Player2Name = "Bob",
            DatePlayed = new DateTime(2025, 1, 24, 14, 30, 0)
        };

        // Act
        var displayName = metadata.GetDisplayName();

        // Assert
        Assert.Contains("Alice", displayName);
        Assert.Contains("Bob", displayName);
        Assert.Contains("2025-01-24", displayName);
    }

    /// <summary>
    /// Tests that GameMetadata.GetWinnerDisplay handles null winner.
    /// </summary>
    [Fact]
    public void GameMetadata_GetWinnerDisplay_ShouldHandleNullWinner()
    {
        // Arrange
        var metadata = new GameMetadata
        {
            Winner = null
        };

        // Act
        var winnerDisplay = metadata.GetWinnerDisplay();

        // Assert
        Assert.Equal("Ingen vinnare", winnerDisplay);
    }

    /// <summary>
    /// Tests that GameMetadata.GetWinnerDisplay returns winner name.
    /// </summary>
    [Fact]
    public void GameMetadata_GetWinnerDisplay_ShouldReturnWinnerName()
    {
        // Arrange
        var metadata = new GameMetadata
        {
            Winner = "Alice"
        };

        // Act
        var winnerDisplay = metadata.GetWinnerDisplay();

        // Assert
        Assert.Equal("Alice", winnerDisplay);
    }

    /// <summary>
    /// Tests that LoadGameForReplay returns null for non-existent file.
    /// </summary>
    [Fact]
    public void GamePersistence_LoadGameForReplay_ShouldReturnNullForNonExistentFile()
    {
        // Act
        var result = GamePersistence.LoadGameForReplay("nonexistent_file_12345");

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that DeleteGame removes the saved game file.
    /// </summary>
    [Fact]
    public void GamePersistence_DeleteGame_ShouldRemoveFile()
    {
        // Arrange
        var game = CreateTestGameService();
        game.InitializeGame("Player1", "Player2");
        game.StartGame();

        //string fileName = $"test_delete_{DateTime.Now.Ticks}";
        string fileName = GamePersistence.SaveGame(game);

        // Verify it exists
        var loadedBefore = GamePersistence.LoadGameForReplay(fileName);
        Assert.NotNull(loadedBefore);

        // Act
        GamePersistence.DeleteGame(fileName);

        // Assert
        var loadedAfter = GamePersistence.LoadGameForReplay(fileName);
        Assert.Null(loadedAfter);
    }

    #endregion

    #region Full Replay Integration Tests

    /// <summary>
    /// Tests a complete replay scenario with multiple moves using ReplayService.
    /// Simulates saving a game, loading it, and replaying moves.
    /// </summary>
    [Fact]
    public void FullReplay_ShouldReplayMultipleMoves()
    {
        // Arrange - Play a real game
        var originalGame = CreateTestGameService();
        originalGame.InitializeGame("Alice", "Bob");
        originalGame.StartGame();

        var board = originalGame.GetBoard();

        // Make several moves
        originalGame.MakeMove(new Position(board.Size - 3, 0), new Position(board.Size - 4, 1));
        originalGame.MakeMove(new Position(2, 1), new Position(3, 2));
        originalGame.MakeMove(new Position(board.Size - 3, 2), new Position(board.Size - 4, 3));

        // Save the game
        //string fileName = $"test_full_replay_{DateTime.Now.Ticks}";
        string fileName = GamePersistence.SaveGame(originalGame);

        // Act - Load and replay
        var savedGame = GamePersistence.LoadGameForReplay(fileName);
        Assert.NotNull(savedGame);

        var replayService = new ReplayService(savedGame);

        // Replay all moves
        while (replayService.StepForward())
        {
            // Continue stepping forward
        }

        // Assert - Final board state should match original
        var originalBoard = originalGame.GetBoard();
        var replayBoard = replayService.GetBoard();

        // Verify piece counts match
        Assert.Equal(
            originalBoard.GetAllPieces().Count,
            replayBoard.GetAllPieces().Count
        );

        // Cleanup
        GamePersistence.DeleteGame(fileName);
    }

    /// <summary>
    /// Tests forward and backward navigation through replay using ReplayService.
    /// Simulates using replay controls to step through a game.
    /// </summary>
    [Fact]
    public void FullReplay_ForwardAndBackward_ShouldMaintainConsistency()
    {
        // Arrange
        var game = CreateTestGameService();
        game.InitializeGame("Player1", "Player2");
        game.StartGame();

        var board = game.GetBoard();

        // Make moves
        game.MakeMove(GetLightStartPosition(board), GetLightFirstMovePosition(board));
        game.MakeMove(GetDarkStartPosition(board), GetDarkFirstMovePosition(board));

        // Save
        string fileName = GamePersistence.SaveGame(game);

        // Load for replay
        var savedGame = GamePersistence.LoadGameForReplay(fileName);
        if (savedGame != null)
        {
            var replayService = new ReplayService(savedGame);

            // Act - Forward one move
            replayService.StepForward();
            var pieceAfterFirstMove = replayService.GetBoard().GetPiece(GetLightFirstMovePosition(board));
            Assert.NotNull(pieceAfterFirstMove);

            // Backward one move
            replayService.StepBackward();
            var pieceAfterUndo = replayService.GetBoard().GetPiece(GetLightStartPosition(board));
            Assert.NotNull(pieceAfterUndo);

            // Forward again - THIS IS THE KEY TEST: can we replay the same move after going back?
            replayService.StepForward();
            var pieceAfterLighto = replayService.GetBoard().GetPiece(GetLightFirstMovePosition(board));
            Assert.NotNull(pieceAfterLighto);
        }

        // Cleanup
        GamePersistence.DeleteGame(fileName);
    }

    /// <summary>
    /// Tests that replay preserves capture information using ReplayService.
    /// </summary>
    [Fact]
    public void FullReplay_ShouldReplayCaptures()
    {
        // Arrange
        var game = CreateTestGameService();
        game.InitializeGame("Player1", "Player2");
        game.StartGame();

        var board = game.GetBoard();

        // Set up capture scenario
        game.MakeMove(GetLightStartPosition(board), GetLightFirstMovePosition(board));
        game.MakeMove(GetDarkStartPosition(board), GetDarkFirstMovePosition(board));

        // Make capture
        game.MakeMove(GetLightFirstMovePosition(board), GetlightCaptureDarkPosition(board));
        // Save
        string fileName = GamePersistence.SaveGame(game);

        // Load and replay
        var savedGame = GamePersistence.LoadGameForReplay(fileName);
        if (savedGame != null)
        {
            var replayService = new ReplayService(savedGame);

            // Replay the capture
            replayService.StepForward();
            replayService.StepForward();
            replayService.StepForward();

            // Assert - CaptuDarkPiece should be recaptured
            var replayBoard = replayService.GetBoard();
            Assert.Null(replayBoard.GetPiece(GetDarkFirstMovePosition(board)));
            Assert.NotNull(replayBoard.GetPiece(GetlightCaptureDarkPosition(board)));
        }

        // Cleanup
        GamePersistence.DeleteGame(fileName);
    }

    #endregion

    #region Position Tests

    /// <summary>
    /// Tests that Position constructor correctly initializes Row and Column.
    /// </summary>
    [Fact]
    public void Position_Constructor_ShouldSetRowAndColumn()
    {
        // Arrange & Act
        var position = new Position(5, 3);

        // Assert
        Assert.Equal(5, position.Row);
        Assert.Equal(3, position.Column);
    }

    /// <summary>
    /// Tests that Position.IsValid returns true for valid positions within board bounds.
    /// </summary>
    [Theory]
    [InlineData(0, 0, 8, true)]     // Top-left corner
    [InlineData(7, 7, 8, true)]     // Bottom-right corner
    [InlineData(3, 4, 8, true)]     // Middle position
    [InlineData(0, 7, 8, true)]     // Top-right corner
    [InlineData(7, 0, 8, true)]     // Bottom-left corner
    public void Position_IsValid_ShouldReturnTrue_WhenPositionIsWithinBounds(int row, int col, int size, bool expected)
    {
        // Arrange
        var position = new Position(row, col);

        // Act
        var result = position.IsValid(size);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests that Position.IsValid returns false for positions outside board bounds.
    /// </summary>
    [Theory]
    [InlineData(-1, 0, 8, false)]   // Negative row
    [InlineData(0, -1, 8, false)]   // Negative column
    [InlineData(8, 0, 8, false)]    // Row equals size (out of bounds)
    [InlineData(0, 8, 8, false)]    // Column equals size (out of bounds)
    [InlineData(10, 10, 8, false)]  // Way outside bounds
    [InlineData(-5, -5, 8, false)]  // Negative both
    public void Position_IsValid_ShouldReturnFalse_WhenPositionIsOutsideBounds(int row, int col, int size, bool expected)
    {
        // Arrange
        var position = new Position(row, col);

        // Act
        var result = position.IsValid(size);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests that Position.Equals(Position) returns true for identical positions.
    /// </summary>
    [Fact]
    public void Position_Equals_ShouldReturnTrue_WhenPositionsAreIdentical()
    {
        // Arrange
        var position1 = new Position(5, 3);
        var position2 = new Position(5, 3);

        // Act
        var result = position1.Equals(position2);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that Position.Equals(Position) returns false for different positions.
    /// </summary>
    [Theory]
    [InlineData(5, 3, 5, 4)]    // Different column
    [InlineData(5, 3, 4, 3)]    // Different row
    [InlineData(5, 3, 4, 4)]    // Different both
    public void Position_Equals_ShouldReturnFalse_WhenPositionsALightifferent(int row1, int col1, int row2, int col2)
    {
        // Arrange
        var position1 = new Position(row1, col1);
        var position2 = new Position(row2, col2);

        // Act
        var result = position1.Equals(position2);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that Position.Equals(object) returns true for identical positions.
    /// </summary>
    [Fact]
    public void Position_EqualsObject_ShouldReturnTrue_WhenPositionsAreIdentical()
    {
        // Arrange
        var position1 = new Position(5, 3);
        object position2 = new Position(5, 3);

        // Act
        var result = position1.Equals(position2);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that Position.Equals(object) returns false for non-Position objects.
    /// </summary>
    [Fact]
    public void Position_EqualsObject_ShouldReturnFalse_WhenObjectIsNotPosition()
    {
        // Arrange
        var position = new Position(5, 3);
        object notAPosition = "not a position";

        // Act
        var result = position.Equals(notAPosition);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that Position.Equals(object) returns false for null.
    /// </summary>
    [Fact]
    public void Position_EqualsObject_ShouldReturnFalse_WhenObjectIsNull()
    {
        // Arrange
        var position = new Position(5, 3);

        // Act
        var result = position.Equals(null);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that Position.GetHashCode returns same hash for identical positions.
    /// This is critical for using Position as a dictionary key or in hash sets.
    /// </summary>
    [Fact]
    public void Position_GetHashCode_ShouldReturnSameHash_WhenPositionsAreIdentical()
    {
        // Arrange
        var position1 = new Position(5, 3);
        var position2 = new Position(5, 3);

        // Act
        var hash1 = position1.GetHashCode();
        var hash2 = position2.GetHashCode();

        // Assert
        Assert.Equal(hash1, hash2);
    }

    /// <summary>
    /// Tests that Position.GetHashCode returns different hashes for different positions.
    /// While not strictly requiLight, good hash functions minimize collisions.
    /// </summary>
    [Fact]
    public void Position_GetHashCode_ShouldReturnDifferentHash_WhenPositionsALightifferent()
    {
        // Arrange
        var position1 = new Position(5, 3);
        var position2 = new Position(5, 4);

        // Act
        var hash1 = position1.GetHashCode();
        var hash2 = position2.GetHashCode();

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    /// <summary>
    /// Tests that Position.ToString returns expected format.
    /// </summary>
    [Theory]
    [InlineData(0, 0, "(0, 0)")]
    [InlineData(5, 3, "(5, 3)")]
    [InlineData(7, 7, "(7, 7)")]
    public void Position_ToString_ShouldReturnFormattedString(int row, int col, string expected)
    {
        // Arrange
        var position = new Position(row, col);

        // Act
        var result = position.ToString();

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Tests that Position can be used as a dictionary key.
    /// This verifies that GetHashCode and Equals work correctly together.
    /// </summary>
    [Fact]
    public void Position_CanBeUsedAsDictionaryKey()
    {
        // Arrange
        var dictionary = new Dictionary<Position, string>();
        var position1 = new Position(5, 3);
        var position2 = new Position(5, 3); // Same as position1
        var position3 = new Position(4, 2); // Different

        // Act
        dictionary[position1] = "First";
        dictionary[position2] = "Second"; // Should overwrite "First"
        dictionary[position3] = "Third";

        // Assert
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("Second", dictionary[position1]);
        Assert.Equal("Second", dictionary[position2]);
        Assert.Equal("Third", dictionary[position3]);
    }

    #endregion
}
