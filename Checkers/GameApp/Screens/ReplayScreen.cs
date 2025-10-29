using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.GameService;
using Checkers.CheckersGame.History;
using Checkers.CheckersGame.Models;
using Checkers.GameApp.Helpers;
using Checkers.GameApp.Screens.Interfaces;
using Checkers.GameApp.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens
{
    /// <summary>
    /// Screen för att replaya sparade spel
    /// </summary>
    public class ReplayScreen : IScreen
    {
        // Variabler för att hålla reda på screens och AppContext
        private readonly IScreenChanger _screenChanger;
        private IAppContext? _appContext;

        // Tjänst för replay av sparade spel
        ReplayService? _replayService;

        // Texturer och färger för brädet
        Texture2D? _lightTexture;
        Texture2D? _darkTexture;
        Color _lightColor = new Color(255, 255, 255);
        Color _darkColor = new Color(34, 32, 52);

        // Texturer för pjäser och UI
        Texture2D? whitePiece;
        Texture2D? whiteKingPiece;
        Texture2D? blackPiece;
        Texture2D? blackKingPiece;
        Texture2D? uiTexture;

        // UI 
        Button? btnMainMenu = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 70, 120, 50), "Main Menu");
        Button? btnAutoPlay = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 130, 120, 50), "Auto Play: ON");
        Button? btnReset = new Button(new Rectangle(MainGame.WindowWidth - 260, MainGame.WindowHeight - 70, 120, 50), "Reset");
        Button? btnNextMove = new Button(new Rectangle(MainGame.WindowWidth - 390, MainGame.WindowHeight - 70, 120, 50), "Next >>");
        Button? btnPreviousMove = new Button(new Rectangle(MainGame.WindowWidth - 520, MainGame.WindowHeight - 70, 120, 50), "<< Previous");
        ComboBox? cboGames = new ComboBox(new Rectangle(MainGame.WindowWidth - 500, 45, 480, 40));
        Label lblGames = new Label(new Rectangle(MainGame.WindowWidth - 500, 10, 480, 100));

        // Checkbox
        CheckBox chkColorBlindMode = new CheckBox(new Rectangle(MainGame.WindowWidth - 500, MainGame.WindowHeight - 115, 200, 50), "Color Blind Mode");

        // Färgblindhet
        Color colorBlindTint = Color.Yellow;
        bool colorBlindMode = false;

        // Board inställningar
        int boardSize = 8;
        int cellSize = 32;
        float boardScale = 1f;
        float drawScale = 1f;

        // Timing inställningar för auto play
        float autoPlayTimer = 500f;
        float lastAutoPlayTime = 0f;
        bool isAutoPlaying = false;

        public ReplayScreen(IScreenChanger screenChanger)
            : base()
        {
            _screenChanger = screenChanger;

            // Subscriba till knapp event handlers
            btnMainMenu.Clicked += BtnMainMenu_Clicked;
            btnNextMove.Clicked += BtnNextMove_Clicked;
            btnPreviousMove.Clicked += BtnPreviousMove_Clicked;
            btnReset.Clicked += BtnReset_Clicked;
            btnAutoPlay.Clicked += btnAutoPlay_Clicked;
            // Subscriba till ComboBox SelectedItemChanged event
            cboGames.SelectedItemChanged += CboGames_SelectedItemChanged;
            // Subscriba till Checkbox CheckedChanged event
            chkColorBlindMode.CheckedChanged += ChkColorBlindMode_CheckedChanged;
        }

        /// <summary>
        /// Hantera ändring av ColorBlindMode checkbox
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void ChkColorBlindMode_CheckedChanged(object? sender, EventArgs e)
        {
            colorBlindMode = chkColorBlindMode.Checked;
        }

        /// <summary>
        /// Toggla auto play läge
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void btnAutoPlay_Clicked(object? sender, EventArgs e)
        {
            // Toggla auto play state
            isAutoPlaying = !isAutoPlaying;

            // Kola om knappen inte är null
            if (btnAutoPlay != null)
            {
                // Uppdatera knappens text beroende på state
                if (isAutoPlaying)
                {
                    btnAutoPlay.Text = "Auto Play: ON";
                }
                else
                {
                    btnAutoPlay.Text = "Auto Play: OFF";
                }
            }
        }

        /// <summary>
        /// Hantera Reset knapp klickande
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnReset_Clicked(object? sender, EventArgs e)
        {
            // Återställ replay till start
            _replayService?.ResetToStart();
        }

        /// <summary>
        /// Hantera Previous Move knapp klickande
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnPreviousMove_Clicked(object? sender, EventArgs e)
        {
            // Gå tillbaka i spelhistoriken om möjligt
            _replayService?.StepBackward();
        }

        /// <summary>
        /// Hantera Next Move knapp klickande
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnNextMove_Clicked(object? sender, EventArgs e)
        {
            // Gå framåt i spelhistoriken om möjligt
            _replayService?.StepForward();
        }

        /// <summary>
        /// Hantera när ett nytt sparat spel väljs i ComboBox
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void CboGames_SelectedItemChanged(object? sender, EventArgs e)
        {
            // Hämta valt filnamn från ComboBox
            string? fileName = cboGames?.SelectedItemText;

            // Kolla så att filnamnet inte är null eller tomt
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            // Hämta sparat spel från GamePersistence
            var savedGame = GamePersistence.LoadGameForReplay(fileName);

            // Om inget sparat spel hittades, returnera
            if (savedGame == null)
            {
                return;
            }

            // Skapa en ny ReplayService med det sparade spelet och återställ till start
            _replayService = new ReplayService(savedGame);
            _replayService.ResetToStart();
        }

        /// <summary>
        /// Hantera Main Menu knapp klickande
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnMainMenu_Clicked(object? sender, EventArgs e)
        {
            // Gå tillbaka till huvudmenyn
            _screenChanger.ChangeScreen(ScreenID.MainMenu);
        }

        public void LoadContent(IAppContext context)
        {
            // Kontrollera att context inte är null
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

            // Skapa texturer för brädet (Svart och Vitt)
            _lightTexture = GraphicsHelper.CreateTexture(context.GraphicsDevice, cellSize, cellSize, _lightColor);
            _darkTexture = GraphicsHelper.CreateTexture(context.GraphicsDevice, cellSize, cellSize, _darkColor);

            // Räkna ut skalning för brädet baserat på fönsterstorlek
            boardScale = (float)MainGame.WindowHeight / (boardSize * cellSize);
            drawScale = (cellSize * boardScale);

            _appContext = context;
            var content = context.Content;

            // Ladda in texturer för pjäser och UI
            whitePiece = content.Load<Texture2D>("White");
            whiteKingPiece = content.Load<Texture2D>("WhiteKing");
            blackPiece = content.Load<Texture2D>("Black");
            blackKingPiece = content.Load<Texture2D>("BlackKing");
            uiTexture = content.Load<Texture2D>("UINormal");

            // Ladda in UI komponenter
            btnMainMenu?.LoadContent(content);
            btnReset?.LoadContent(content);
            btnNextMove?.LoadContent(content);
            btnPreviousMove?.LoadContent(content);
            btnAutoPlay?.LoadContent(content);
            lblGames.LoadContent(content);
            lblGames.Text = "Previous Games";
            lblGames.FontColor = Color.White;
            chkColorBlindMode.LoadContent(content);

            // Säkerställ att cboGames inte är null innan vi laddar in innehåll
            if (cboGames != null)
            {
                // Ladda in och sätt standardvärde för ComboBox
                cboGames.LoadContent(content);
                cboGames.SelectedItemText = "<Select a Previous Game>";

                // Ladda in sparade spel för replay
                var savedGames = GamePersistence.GetSavedGamesWithMetadata();
                foreach (var game in savedGames)
                {
                    // Lägg till varje sparat spel i ComboBox
                    cboGames.AddItem(game.FileName);
                }
            }
        }

        public void UnloadContent()
        {
            // Töm resurser och återställ variabler
            _replayService = null;
            btnMainMenu?.UnloadContent();
            btnReset?.UnloadContent();
            btnNextMove?.UnloadContent();
            btnPreviousMove?.UnloadContent();
            btnAutoPlay?.UnloadContent();
            cboGames?.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            // Uppdatera knappar och ComboBox
            btnMainMenu?.Update(gameTime);
            btnReset?.Update(gameTime);
            btnNextMove?.Update(gameTime);
            btnPreviousMove?.Update(gameTime);
            btnAutoPlay?.Update(gameTime);
            cboGames?.Update(gameTime);
            chkColorBlindMode?.Update(gameTime);

            // Uppdatera inget om replay service är null
            if (_replayService == null)
            {
                return;
            }

            // Uppdatera knappars enabled state baserat på replay service state
            if (btnNextMove != null)
            {
                btnNextMove.Enabled = _replayService != null && !_replayService.IsAtEnd && !isAutoPlaying;
            }

            if (btnPreviousMove != null)
            {
                btnPreviousMove.Enabled = _replayService != null && !_replayService.IsAtStart && !isAutoPlaying;
            }

            if (btnAutoPlay != null)
            {
                btnAutoPlay.Enabled = _replayService != null;
            }

            // Hantera auto play logik
            if (isAutoPlaying)
            {
                // Hämta tid sedan senaste uppdatering
                lastAutoPlayTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // Kolla om tiden sedan senaste uppdatering är större än auto play timer
                if (lastAutoPlayTime >= autoPlayTimer)
                {
                    // Nollställ timer och gå till nästa steg i replay
                    lastAutoPlayTime = 0f;

                    // Gå framåt i replay om möjligt och vi inte är i slutet av spelet
                    if (_replayService != null && !_replayService.IsAtEnd)
                    {
                        _replayService.StepForward();
                    }
                    else
                    {
                        // Annars, stäng av auto play
                        isAutoPlaying = false;

                        // Uppdatera knappens text 
                        if (btnAutoPlay != null)
                        {
                            btnAutoPlay.Text = "Auto Play: OFF";
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            // Felhantering för AppContext och SpriteBatch
            // Om något är null så ska inget försökas ritas ut och vi hoppar över drawcall
            if (_appContext == null)
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
                    // Välj färg och textur baserat på positionens parity
                    Color cellColor = ((x + y) % 2 == 0) ? _lightColor : _darkColor;
                    Texture2D? cellTexture = ((x + y) % 2 == 0) ? _lightTexture : _darkTexture;

                    if (cellTexture == null)
                    {
                        return;
                    }

                    // Rita cellen
                    spriteBatch.Draw(cellTexture, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, cellColor, 0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }

            // Om replay service inte är null
            if (_replayService != null)
            {
                // Hämta brädet och alla pjäser
                var board = _replayService.GetBoard();
                var pieces = board?.GetAllPieces();

                // Om pjäser finns
                if (pieces != null)
                {
                    Color drawColor = colorBlindMode ? colorBlindTint : Color.White;
                    // Gå igenom varje pjäs
                    for (int i = 0; i < pieces.Count; i++)
                    {
                        // Initiera en pjäs och dess textur
                        var piece = pieces[i];
                        Texture2D? pieceTexture = null;

                        // Hämta rätt textur baserat på pjäsens färg och typ
                        if (piece.Color == PieceColor.Light)
                        {
                            pieceTexture = piece is KingPiece ? whiteKingPiece : whitePiece;
                        }
                        else
                        {
                            pieceTexture = piece is KingPiece ? blackKingPiece : blackPiece;
                        }

                        // Rita pjäsen på dess position
                        spriteBatch.Draw(pieceTexture, new Rectangle((int)(piece.Position.Column * drawScale), (int)(piece.Position.Row * drawScale), (int)drawScale, (int)drawScale), null, drawColor, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    }
                }
            }

            // Rita en border för spelområdet
            if (uiTexture != null)
            {
                spriteBatch.Draw(uiTexture, new Rectangle((int)(boardSize * drawScale), 0, 3 * 3, MainGame.WindowHeight), new Rectangle(0, 7, 3, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
            }
            spriteBatch.End();

            // Rita UI komponenter
            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnMainMenu?.Draw(spriteBatch);
            btnReset?.Draw(spriteBatch);
            btnNextMove?.Draw(spriteBatch);
            btnPreviousMove?.Draw(spriteBatch);
            btnAutoPlay?.Draw(spriteBatch);
            lblGames?.Draw(spriteBatch);
            cboGames?.Draw(spriteBatch);
            chkColorBlindMode?.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
