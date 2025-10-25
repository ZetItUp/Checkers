using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.GameService;
using Checkers.UI;
using Checkers.GameApp.Helpers;
using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.Models;
using Checkers.CheckersGame.History;

namespace Checkers.GameApp.Screens
{
    public class GameScreen : Screen
    {
        Texture2D _lightTexture;
        Texture2D _darkTexture;
        Color _lightColor = new Color(255, 255, 255);
        Color _darkColor = new Color(34, 32, 52);
        GameService? _gameService;

        Texture2D whitePiece;
        Texture2D whiteKingPiece;
        Texture2D blackPiece;
        Texture2D blackKingPiece;

        Texture2D selectTexture;
        Texture2D validMoveTexture;
        Texture2D uiTexture;

        Button btnMainMenu = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 70, 120, 50), "Main Menu");
        Button btnStartGame = new Button(new Rectangle(MainGame.WindowWidth - 260, MainGame.WindowHeight - 70, 120, 50), "Start Game");
        Button btnUndoMove = new Button(new Rectangle(MainGame.WindowWidth - 390, MainGame.WindowHeight - 70, 120, 50), "Undo Move");
        Button btnRestartGame = new Button(new Rectangle(MainGame.WindowWidth - 260, MainGame.WindowHeight - 70, 120, 50), "Restart Game");
        Button btnEndTurn = new Button(new Rectangle(MainGame.WindowWidth - 520, MainGame.WindowHeight - 70, 120, 50), "End Turn");
        Button btnSaveGame = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 130, 120, 50), "Save Game"); 

        bool isPieceSelected = false;
        bool isInMultiJumpMode = false; // tracker för att kolla om vi är i ett multi-jump

        int boardSize = 0;
        int cellSize = 32;
        float boardScale = 1f;
        float drawScale = 1f;

        Position selectedPosition;
        Player currentPlayer;
        List<Position> validMoves = new List<Position>();

        public GameScreen()
            : base()
        {
            btnMainMenu.Clicked += BtnTest_Clicked;
            btnStartGame.Clicked += BtnStartGame_Clicked;
            btnUndoMove.Enabled = false;
            btnUndoMove.Clicked += BtnUndoMove_Clicked;
            btnRestartGame.Clicked += BtnRestartGame_Clicked;
            btnEndTurn.Clicked += BtnEndTurn_Clicked;
            btnSaveGame.Clicked += BtnSaveGame_Clicked;
        }

        private void BtnEndTurn_Clicked(object? sender, EventArgs e)
        {
            _gameService?.EndTurn();
            isPieceSelected = false;
            validMoves.Clear();
            isInMultiJumpMode = false;
        }

        private void BtnUndoMove_Clicked(object? sender, EventArgs e)
        {
            _gameService?.Undo();
        }

        private void BtnStartGame_Clicked(object? sender, EventArgs e)
        {
            _gameService?.StartGame();
            btnStartGame.Enabled = false;
            btnUndoMove.Enabled = false;
            btnStartGame.IsVisible = false;
            btnRestartGame.IsVisible = true;
            btnRestartGame.Enabled = true;
        }

        private void BtnTest_Clicked(object sender, EventArgs e)
        {
            // Gå tillbaka till huvudmenyn
            ScreenManager.ChangeScreen(ScreenID.MainMenu);
        }

        private void BtnRestartGame_Clicked(object? sender, EventArgs e)
        {
            if (_gameService != null) // null = spelet är inte igång
            {
                _gameService.InitializeGame("Player 1", "Player 2"); // laddar om spelet
                _gameService.StartGame();

                // resetar UI
                isPieceSelected = false; // ingen pjäs är vald
                validMoves.Clear(); // tidigare beräknade drag raderas
            }
        }

        private void BtnSaveGame_Clicked(object? sender, EventArgs e)
        {
            if (_gameService != null)
            {
                try
                {
                    string fileName = GamePersistence.SaveGame(_gameService);
                    Console.WriteLine($"Game saved successfully as: {fileName}.json");
                    Console.WriteLine($"Save location: {System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving game: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        public override void LoadContent(ContentManager content)
        {
            _gameService = new GameService();
            boardSize = _gameService.RuleSet.BoardSize;
            _lightTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, _lightColor);
            _darkTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, _darkColor);

            // Set board scale to fit window height
            boardScale = (float)MainGame.WindowHeight / (boardSize * cellSize);
            drawScale = (cellSize * boardScale);

            whitePiece = content.Load<Texture2D>("White");
            whiteKingPiece = content.Load<Texture2D>("WhiteKing");
            blackPiece = content.Load<Texture2D>("Black");
            blackKingPiece = content.Load<Texture2D>("BlackKing");

            validMoveTexture = content.Load<Texture2D>("Move");
            uiTexture = content.Load<Texture2D>("UINormal");
            selectTexture = content.Load<Texture2D>("Select");

            btnMainMenu.LoadContent(content);
            btnStartGame.LoadContent(content);
            btnUndoMove.LoadContent(content);
            btnRestartGame.LoadContent(content);
            btnEndTurn.LoadContent(content);
            btnSaveGame.LoadContent(content);

            btnRestartGame.Enabled = false;
            btnRestartGame.IsVisible = false;
            btnStartGame.Enabled = true;
            btnStartGame.IsVisible = true;
            btnEndTurn.Enabled = false;
            btnEndTurn.IsVisible = false;
            btnSaveGame.Enabled = true;

            _gameService.InitializeGame("Player 1", "Player 2");
        }
        public override void UnloadContent()
        {
            _gameService = null;
        }

        public override void Update(GameTime gameTime)
        {
            btnMainMenu.Update(gameTime);
            btnStartGame.Update(gameTime);
            btnUndoMove.Update(gameTime);
            btnRestartGame.Update(gameTime);
            btnEndTurn.Update(gameTime);
            btnSaveGame.Update(gameTime);

            if (_gameService == null || _gameService.GetGameStatus() != GameStatus.InProgress)
            {
                return;
            }

            var history = _gameService.GetGameHistory();
            btnUndoMove.Enabled = history != null && history.GetAllMoves().Count > 0;

            // Visa End Turn knappen bara om ForcedCaptures är av
            btnEndTurn.IsVisible = !_gameService.RuleSet.ForcedCaptures;
            btnEndTurn.Enabled = !_gameService.RuleSet.ForcedCaptures && isPieceSelected;

            currentPlayer = _gameService.GetCurrentPlayer();

            var mousePositionX = (int)(MouseHelper.MousePosition().X / drawScale);
            var mousePositionY = (int)(MouseHelper.MousePosition().Y / drawScale);

            if (MouseHelper.MousePressed(MouseHelper.MouseButton.Left))
            {
                var hoveredPiece = _gameService.GetBoard().GetPiece(new Position(mousePositionY, mousePositionX));
                if (hoveredPiece != null)
                {
                    if(currentPlayer.Color != hoveredPiece.Color)
                    {
                        isPieceSelected = false;
                        return;
                    }

                    // Om vi är i multi-jump läge, tillåt inte byte av pjäs
                    if (isInMultiJumpMode && (mousePositionY != selectedPosition.Row || mousePositionX != selectedPosition.Column))
                    {
                        return; // Måste fortsätta med samma pjäs i multi-jump
                    }

                    isPieceSelected = true;
                    selectedPosition = new Position(mousePositionY, mousePositionX);
                    // Använd GameService för att få bara GILTIGA drag
                    validMoves = _gameService.GetValidMovesForPiece(selectedPosition);
                }
                else
                {
                    foreach(var move in validMoves)
                    {
                        if(move.Row == mousePositionY && move.Column == mousePositionX)
                        {
                            // Gör draget och spara position
                            var moveToPosition = move;
                            var previousPlayer = _gameService.GetCurrentPlayer();
                            _gameService.MakeMove(selectedPosition, move);

                            // Kolla om samma spelare fortfarande är i tur (betyder multi-jump möjligt)
                            var newPlayer = _gameService.GetCurrentPlayer();
                            if (previousPlayer.Color == newPlayer.Color)
                            {
                                // Spelaren kan ta igen! Auto-select pjäsen och aktivera multi-jump läge
                                selectedPosition = moveToPosition;
                                validMoves = _gameService.GetValidMovesForPiece(selectedPosition);
                                isPieceSelected = true;
                                isInMultiJumpMode = true; // Nu är vi i multi-jump läge
                            }
                            else
                            {
                                // Normal turn switch - rensa selection
                                validMoves.Clear();
                                isPieceSelected = false;
                                isInMultiJumpMode = false;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);
            
            // Rita ett schackbräde
            for (int y = 0; y < boardSize; y++)
            {
                for(int x = 0; x < boardSize; x++)
                {
                    Color cellColor = ((x + y) % 2 == 0) ? _lightColor : _darkColor;
                    Texture2D cellTexture = ((x + y) % 2 == 0) ? _lightTexture : _darkTexture;
                    
                    spriteBatch.Draw(cellTexture, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, cellColor, 0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }

            if (_gameService != null && _gameService.GetGameStatus() == GameStatus.InProgress)
            {
                var pieces = _gameService.GetBoard().GetAllPieces();

                for (int i = 0; i < pieces.Count; i++)
                {
                    var piece = pieces[i];
                    Texture2D pieceTexture = null;
                    if (piece.Color == PieceColor.Red)
                    {
                        pieceTexture = piece is KingPiece ? whiteKingPiece : whitePiece;
                    }
                    else
                    {
                        pieceTexture = piece is KingPiece ? blackKingPiece : blackPiece;
                    }
                    spriteBatch.Draw(pieceTexture, new Rectangle((int)(piece.Position.Column * drawScale), (int)(piece.Position.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                }

                if (isPieceSelected)
                {
                    spriteBatch.Draw(selectTexture, new Rectangle((int)(selectedPosition.Column * drawScale), (int)(selectedPosition.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);

                    foreach (var move in validMoves)
                    {
                        spriteBatch.Draw(validMoveTexture, new Rectangle((int)(move.Column * drawScale), (int)(move.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.7f);
                    }
                }
            }

            spriteBatch.Draw(uiTexture, new Rectangle((int)(_gameService.RuleSet.BoardSize * drawScale), 0, 3 * 3, MainGame.WindowHeight), new Rectangle(0, 7, 3, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnMainMenu.Draw(spriteBatch);
            btnStartGame.Draw(spriteBatch);
            btnUndoMove.Draw(spriteBatch);
            btnRestartGame.Draw(spriteBatch);
            btnEndTurn.Draw(spriteBatch);
            btnSaveGame.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
