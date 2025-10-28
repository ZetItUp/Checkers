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
using Checkers.GameApp.Screens.Interfaces;

namespace Checkers.GameApp.Screens
{
    /// <summary>
    /// Screen för spelet
    /// </summary>
    public class GameScreen : IScreen
    {
        // Variabler för att hålla reda på screens och AppContext
        private readonly IScreenChanger _screenChanger;
        private IAppContext? _appContext;

        // GameService 
        GameService? _gameService;

        // Texturer
        Texture2D? _lightTexture;
        Texture2D? _darkTexture;

        Texture2D? whitePiece;
        Texture2D? whiteKingPiece;
        Texture2D? blackPiece;
        Texture2D? blackKingPiece;

        Texture2D? selectTexture;
        Texture2D? validMoveTexture;
        Texture2D? uiTexture;

        // Knappar 
        Button btnMainMenu = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 70, 120, 50), "Main Menu");
        Button btnStartGame = new Button(new Rectangle(MainGame.WindowWidth - 260, MainGame.WindowHeight - 70, 120, 50), "Start Game");
        Button btnUndoMove = new Button(new Rectangle(MainGame.WindowWidth - 390, MainGame.WindowHeight - 70, 120, 50), "Undo Move");
        Button btnRestartGame = new Button(new Rectangle(MainGame.WindowWidth - 260, MainGame.WindowHeight - 70, 120, 50), "Restart Game");
        Button btnEndTurn = new Button(new Rectangle(MainGame.WindowWidth - 520, MainGame.WindowHeight - 70, 120, 50), "End Turn");
        Button btnSaveGame = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 130, 120, 50), "Save Game");

        // Variabler för att hantera om en pjäs är markerad och om man måste flytta igen
        bool isPieceSelected = false;
        bool isInMultiJumpMode = false;
        bool isWinner = false;

        // Spelbrädes variabler för scaling och size
        int boardSize = 0;
        int cellSize = 32;
        float boardScale = 1f;
        float drawScale = 1f;

        // Färger som används vid ritning av texturer
        Color _lightColor = new Color(255, 255, 255);
        Color _darkColor = new Color(34, 32, 52);

        // Vald position 
        Position selectedPosition;
        // Nuvarande Spelare
        Player? currentPlayer;

        // Lista på tillgängliga moves som en pjäs kan utföra
        List<Position> validMoves = new List<Position>();

        public GameScreen(IScreenChanger screenChanger)
            : base()
        {
            // Sätt screenChanger
            _screenChanger = screenChanger;

            // Subscribe:a till alla knappars Clicked event
            btnMainMenu.Clicked += BtnMainMenu_Clicked;
            btnStartGame.Clicked += BtnStartGame_Clicked;
            btnUndoMove.Clicked += BtnUndoMove_Clicked;
            btnRestartGame.Clicked += BtnRestartGame_Clicked;
            btnEndTurn.Clicked += BtnEndTurn_Clicked;
            btnSaveGame.Clicked += BtnSaveGame_Clicked;

            // Disable:a Undo knappen
            btnUndoMove.Enabled = false;
        }

        /// <summary>
        /// Hantera knapptryck för EndTurn click
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnEndTurn_Clicked(object? sender, EventArgs e)
        {
            // Säg till GameService att spelaren valt att avsluta sin turn
            _gameService?.EndTurn();
            // Avmarkera vald pjäs
            isPieceSelected = false;
            // Rensa validMoves listan
            validMoves.Clear();
            // Vi är inte längre i MultiJumpMode
            isInMultiJumpMode = false;
        }

        /// <summary>
        /// Hantera knapptryckning för UndoMove click
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnUndoMove_Clicked(object? sender, EventArgs e)
        {
            // Säg till GameService att ångra senaste draget
            _gameService?.Undo();
        }

        /// <summary>
        /// Hantera knapptryckning för StartGame click
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnStartGame_Clicked(object? sender, EventArgs e)
        {
            // Säg åt GameService att starta ett spel
            _gameService?.StartGame();
            // Disable:a och göm knappar som inte längre behövs
            btnStartGame.Enabled = false;
            btnUndoMove.Enabled = false;
            btnStartGame.IsVisible = false;
            // Visa och enable:a knappar som behövs
            btnRestartGame.IsVisible = true;
            btnRestartGame.Enabled = true;
        }

        /// <summary>
        /// Hantera knapptryckning för MainMenu click
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnMainMenu_Clicked(object? sender, EventArgs e)
        {
            // Gå tillbaka till huvudmenyn
            _screenChanger.ChangeScreen(ScreenID.MainMenu);
        }

        /// <summary>
        /// Hantera knapptryckning för RestartGame click
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnRestartGame_Clicked(object? sender, EventArgs e)
        {
            // Kontrollera att _gameService inte är null
            if (_gameService != null)
            {
                // Initiera ett nytt spel
                _gameService.InitializeGame("Player 1", "Player 2");
                // Starta ett nytt spel
                _gameService.StartGame();

                // Resetar UI
                isPieceSelected = false;
                // Rensa validMoves listan
                validMoves.Clear();
            }
        }

        /// <summary>
        /// Hantera knapptryckning för SaveGame click
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnSaveGame_Clicked(object? sender, EventArgs e)
        {
            // Kontrollera att _gameService inte är null
            if (_gameService != null)
            {
                try
                {
                    // Försök spara spelet till en json-fil
                    string fileName = GamePersistence.SaveGame(_gameService);

                    // Logga console om det lyckades och vart filen sparades
                    Console.WriteLine($"Game saved successfully as: {fileName}.json");
                    Console.WriteLine($"Save location: {System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves")}");
                }
                catch (Exception ex)
                {
                    // Logga fel i console
                    Console.WriteLine($"Error saving game: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        public void LoadContent(IAppContext context)
        {
            if (context == null)
            {
                // Kasta exception, händer detta så har inladdningen misslyckats i MainGame.cs, detta ska inte ske.
                throw new Exception("IAppContext is not initialized.");
            }

            // Kontrollera att GraphicsDevice inte är null
            if (context.GraphicsDevice == null)
            {

                // Kasta exception, händer detta så har inladdningen misslyckats i MainGame.cs, detta ska inte ske.
                throw new Exception("GraphicsDevice is not initialized.");
            }

            // Kontrollera att ContentManager inte är null
            if (context.Content == null)
            {
                // Kasta exception, händer detta så har inladdningen misslyckats i MainGame.cs, detta ska inte ske.
                throw new Exception("ContentManager is not initialized.");
            }

            // Skapa en GameService 
            _gameService = new GameService();
            _appContext = context;

            // Initiera ett nytt spel
            if (!_gameService.InitializeGame("Player 1", "Player 2"))
            {
                _screenChanger.ChangeScreen(ScreenID.MainMenu);
                return;
            }

            if (_gameService.RuleSet != null)
            {
                // Hämta board size
                boardSize = _gameService.RuleSet.BoardSize;
            }

            // Skapa texturer för brädet, en svart och en vit, med cellSize storlek
            _lightTexture = GraphicsHelper.CreateTexture(context.GraphicsDevice, cellSize, cellSize, _lightColor);
            _darkTexture = GraphicsHelper.CreateTexture(context.GraphicsDevice, cellSize, cellSize, _darkColor);

            // Beräkna scaling så vi kan rita ut brädet anpassat efter fönstrets storlek och board size
            boardScale = (float)MainGame.WindowHeight / (boardSize * cellSize);
            drawScale = (cellSize * boardScale);

            var content = context.Content;

            // Ladda texturer
            whitePiece = content.Load<Texture2D>("White");
            whiteKingPiece = content.Load<Texture2D>("WhiteKing");
            blackPiece = content.Load<Texture2D>("Black");
            blackKingPiece = content.Load<Texture2D>("BlackKing");
            validMoveTexture = content.Load<Texture2D>("Move");
            uiTexture = content.Load<Texture2D>("UINormal");
            selectTexture = content.Load<Texture2D>("Select");

            // Ladda alla knappar
            btnMainMenu.LoadContent(content);
            btnStartGame.LoadContent(content);
            btnUndoMove.LoadContent(content);
            btnRestartGame.LoadContent(content);
            btnEndTurn.LoadContent(content);
            btnSaveGame.LoadContent(content);

            // Ladda ljud
            Sound.LoadContent(content);

            // Ställ in alla knappars default synlighet och om dom är aktiva
            btnRestartGame.Enabled = false;
            btnRestartGame.IsVisible = false;
            btnStartGame.Enabled = true;
            btnStartGame.IsVisible = true;
            btnEndTurn.Enabled = false;
            btnEndTurn.IsVisible = false;
            btnSaveGame.Enabled = true;

        }
        public void UnloadContent()
        {
            // Töm resurser
            _gameService = null;
            btnEndTurn?.UnloadContent();
            btnStartGame?.UnloadContent();
            btnUndoMove?.UnloadContent();
            btnRestartGame?.UnloadContent();
            btnSaveGame?.UnloadContent();
            btnMainMenu?.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (isWinner)
            {
                return;

            }
            // Uppdatera knapparna
            btnMainMenu.Update(gameTime);
            btnStartGame.Update(gameTime);
            btnUndoMove.Update(gameTime);
            btnRestartGame.Update(gameTime);
            btnEndTurn.Update(gameTime);
            btnSaveGame.Update(gameTime);


            // Uppdatera inget annat om _gameService är null eller om ett spel inte är GameStatus.InProgress
            if (_gameService == null || _gameService.RuleSet == null || _gameService.GetGameStatus() != GameStatus.InProgress)
            {
                return;
            }

            // Hämta spelhistoriken från nuvarande spel
            var history = _gameService.GetGameHistory();
            // Uppdatera UndoMove knappen beroende på history state eller om det finns några moves gjorda
            btnUndoMove.Enabled = history != null && history.GetAllMoves().Count > 0;

            // Visa EndTurn knappen bara om ForcedCaptures är av
            btnEndTurn.IsVisible = !_gameService.RuleSet.ForcedCaptures;
            btnEndTurn.Enabled = !_gameService.RuleSet.ForcedCaptures && isPieceSelected;

            // Hämta currentPlayer från _gameService
            currentPlayer = _gameService.GetCurrentPlayer();

            // Räkna ut musens position på brädet
            // Vi delar med drawScale för att få exakta rutan som musen är över
            var mousePositionX = (int)(MouseHelper.MousePosition().X / drawScale);
            var mousePositionY = (int)(MouseHelper.MousePosition().Y / drawScale);

            // Kolla om musen är nedtryckt
            if (MouseHelper.MousePressed(MouseButton.Left))
            {
                // Hämta board
                var board = _gameService.GetBoard();
                // Försök hämta en pjäs från board där musen är
                var hoveredPiece = board?.GetPiece(new Position(mousePositionY, mousePositionX));

                // Om pjäsen inte är null
                if (hoveredPiece != null)
                {
                    // Kontrollera att vi har en currentPlayer och att currentPlayers färg är samma som sig själv
                    if (currentPlayer != null && currentPlayer.Color != hoveredPiece.Color)
                    {
                        // Avmarkera pjäsen och returnera
                        isPieceSelected = false;
                        return;
                    }

                    // Om vi är i multi-jump läge, tillåt inte byte av pjäs
                    if (isInMultiJumpMode && (mousePositionY != selectedPosition.Row || mousePositionX != selectedPosition.Column))
                    {
                        return; // Måste fortsätta med samma pjäs i multi-jump
                    }

                    // Markera en pjäs
                    isPieceSelected = true;
                    // Spara positionen på markerad pjäs
                    selectedPosition = new Position(mousePositionY, mousePositionX);
                    // Använd GameService för att få giltiga drag
                    validMoves = _gameService.GetValidMovesForPiece(selectedPosition);
                }
                // Om pjäsen är null
                else
                {
                    // Loopa igenom alla validMoves
                    foreach (var move in validMoves)
                    {
                        // Kontrollera så att draget som görs är på samma plats som musens position
                        if (move.Row == mousePositionY && move.Column == mousePositionX)
                        {
                            // Gör draget och spara position
                            var moveToPosition = move;
                            var previousPlayer = _gameService.GetCurrentPlayer();
                            _gameService.MakeMove(selectedPosition, move);

                            // Kolla om samma spelare fortfarande är i tur (betyder multi-jump möjligt)
                            var newPlayer = _gameService.GetCurrentPlayer();
                            if (previousPlayer?.Color == newPlayer?.Color)
                            {
                                // Spelaren kan ta igen! Auto-select pjäsen och aktivera multi-jump läge
                                selectedPosition = moveToPosition;
                                // Hämta giltiga drag
                                validMoves = _gameService.GetValidMovesForPiece(selectedPosition);
                                // Markera pjäsen
                                isPieceSelected = true;
                                // Nu är vi i multi-jump läge
                                isInMultiJumpMode = true;
                            }
                            else
                            {
                                // Vanligt drag
                                // Rensa validMoves
                                validMoves.Clear();
                                // Avmarkera pjäs
                                isPieceSelected = false;
                                // Se till att vi inte är i multi-jump läge
                                isInMultiJumpMode = false;
                            }
                            break;
                        }
                    }
                }
            }

            // Kolla om någon har vunnit spelet ännu
            if (_gameService.CheckWinner() != null)
            {
                isWinner = true;
                Sound.PlayWinSound();
            }

        }

        public void Draw(GameTime gameTime)
        {
            // Felhantering för AppContext och SpriteBatch
            // Om något är null så ska inget försökas ritas ut och vi hoppar över drawcall
            if(_appContext == null)
            {
                return;
            }

            var spriteBatch = _appContext.SpriteBatch;

            if (spriteBatch == null)
            {
                return;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            // Rita ett schackbräde
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    // Hämta färg baserat på position, varannan svart, varannan vit
                    Color cellColor = ((x + y) % 2 == 0) ? _lightColor : _darkColor;
                    Texture2D? cellTexture = ((x + y) % 2 == 0) ? _lightTexture : _darkTexture;

                    // Rita ut cellen
                    spriteBatch.Draw(cellTexture, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, cellColor, 0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }

            // Rita bara pjäser om _gameService inte är null och ett game är i GameStatus.InProgress
            if (_gameService != null && _gameService.GetGameStatus() == GameStatus.InProgress)
            {
                // Hämta brädet från GameService
                var board = _gameService.GetBoard();
                // hämta alla pjäser från brädet
                var pieces = board?.GetAllPieces();

                // Kontrollera att det finns pjäser
                if (pieces != null)
                {
                    // Loopa igenom alla pjäser
                    for (int i = 0; i < pieces.Count; i++)
                    {
                        // Hämta nuvarande pjäs
                        var piece = pieces[i];
                        Texture2D? pieceTexture = null;

                        // Sätt dess texture baserat på vilken färg pjäsen är
                        if (piece.Color == PieceColor.Red)
                        {
                            pieceTexture = piece is KingPiece ? whiteKingPiece : whitePiece;
                        }
                        else
                        {
                            pieceTexture = piece is KingPiece ? blackKingPiece : blackPiece;
                        }

                        // Rita ut pjäsen på dess position med dess texture
                        spriteBatch.Draw(pieceTexture, new Rectangle((int)(piece.Position.Column * drawScale), (int)(piece.Position.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    }
                }

                // Om en pjäs är markerad
                if (isPieceSelected)
                {
                    // Rita ut markering
                    spriteBatch.Draw(selectTexture, new Rectangle((int)(selectedPosition.Column * drawScale), (int)(selectedPosition.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.8f);

                    // Loopa igenom alla validMoves
                    foreach (var move in validMoves)
                    {
                        // Rita en markör som visar alla giltiga moves pjäsen kan göra baserat på dess position och omgivning
                        spriteBatch.Draw(validMoveTexture, new Rectangle((int)(move.Column * drawScale), (int)(move.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.7f);
                    }
                }
            }

            if (_gameService != null)
            {
                // Rita ut en border för spelbrädet där det tar slut
                spriteBatch.Draw(uiTexture, new Rectangle((int)(_gameService.RuleSet!.BoardSize * drawScale), 0, 3 * 3, MainGame.WindowHeight), new Rectangle(0, 7, 3, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            // Rita alla knappar
            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnMainMenu.Draw(spriteBatch);
            btnStartGame.Draw(spriteBatch);
            btnUndoMove.Draw(spriteBatch);
            btnRestartGame.Draw(spriteBatch);
            btnEndTurn.Draw(spriteBatch);
            btnSaveGame.Draw(spriteBatch);
            spriteBatch.End();

            // rita victory-screen
            /* if (isWinner)
             {
                 spriteBatch.Draw(uiTexture, new Rectangle(currX, currY, 6, 6), new Rectangle(0, 0, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX + 6, currY, WindowRectangle.Width - 12, 6), new Rectangle(6, 0, 1, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY, 6, 6), new Rectangle(activeTexture.Width - 6, 0, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(0, 6, 6, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX + 6, currY + 6, WindowRectangle.Width - 12, WindowRectangle.Height - 12), new Rectangle(6, 6, 1, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(activeTexture.Width - 6, 6, 6, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(0, activeTexture.Height - 6, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX + 6, currY + WindowRectangle.Height - 6, WindowRectangle.Width - 12, 6), new Rectangle(6, activeTexture.Height - 6, 1, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                 spriteBatch.Draw(uiTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(activeTexture.Width - 6, activeTexture.Height - 6, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);

             }
            */
        }
    }
}
