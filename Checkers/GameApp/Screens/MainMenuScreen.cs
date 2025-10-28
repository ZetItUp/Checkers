using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.GameApp.Screens.Interfaces;
using Checkers.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Checkers.GameApp.Screens
{
    /// <summary>
    /// Screen för huvudmenyn
    /// </summary>
    public class MainMenuScreen : IScreen
    {
        // Variabler för att hålla reda på screens och AppContext
        private readonly IScreenChanger _screenChanger;
        private IAppContext? _appContext;

        // Texture för bakgrunden
        Texture2D? background;

        // Skalningsfaktor och bredd för bakgrunden
        float bgScale = 2.0f;
        int bgWidth = 0;

        // Knappar för menyn
        Button btnStartGame = new Button(new Rectangle(MainGame.WindowWidth / 2 - (250 / 2), MainGame.WindowHeight / 2 - 50, 250, 80), "Start Game");
        Button btnReplayGames= new Button(new Rectangle(MainGame.WindowWidth / 2 - (250 / 2), MainGame.WindowHeight / 2 - 50 + 100, 250, 80), "Replay Games");
        Button btnExitGame = new Button(new Rectangle(MainGame.WindowWidth - 160, MainGame.WindowHeight - 70, 140, 50), "Exit Game");

        public MainMenuScreen(IScreenChanger screenChanger)
            : base()
        {
            _screenChanger = screenChanger;

            // Subscribe:a till knapparnas Clicked event
            btnStartGame.Clicked += BtnStartGame_Clicked;
            btnExitGame.Clicked += BtnExitGame_Clicked;
            btnReplayGames.Clicked += BtnReplayGames_Clicked;
        }

        /// <summary>
        /// Hantera knapptryckning för ReplayGames
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnReplayGames_Clicked(object? sender, EventArgs e)
        {
            // Byt screen till ReplayScreen
            _screenChanger.ChangeScreen(ScreenID.Replay);
        }

        /// <summary>
        /// Hantera knapptryckning för ExitGame
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnExitGame_Clicked(object? sender, EventArgs e)
        {
            // Sätt ExitGame till true för att avsluta spelet
            MainGame.ExitGame = true;
        }

        /// <summary>
        /// Hantera knapptryckning för StartGame
        /// </summary>
        /// <param name="sender">Ej använt</param>
        /// <param name="e">Ej använt</param>
        private void BtnStartGame_Clicked(object? sender, EventArgs e)
        {
            // Byt till GameScreen
            _screenChanger.ChangeScreen(ScreenID.Game);
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

            _appContext = context;
            var content = context.Content;

            // Ladda bakgrundstexturen
            background = content.Load<Texture2D>("Checkers");

            if(background == null)
            {
                MainGame.ExitGame = true;
                return;
            }

            // Ställ in scaling baserat på bakgrunden
            bgWidth = (int)(background.Width * bgScale);

            // Ladda knapparna
            btnStartGame?.LoadContent(content);
            btnExitGame?.LoadContent(content);
            btnReplayGames?.LoadContent(content);
        }

        public void UnloadContent()
        {
            // Töm resurser
            btnExitGame?.UnloadContent();
            btnReplayGames?.UnloadContent();
            btnStartGame?.UnloadContent();
            background = null;
        }

        public void Update(GameTime gameTime)
        {
            // Uppdatera knapparna
            btnStartGame?.Update(gameTime);
            btnReplayGames?.Update(gameTime);
            btnExitGame?.Update(gameTime);
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

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap);

            // Kolla så att bakgrunden existerar
            if (background != null)
            {
                // Rita ut bakgrunden centrerad mot toppen
                spriteBatch.Draw(background, new Rectangle(MainGame.WindowWidth / 2 - bgWidth / 2, 0, bgWidth, (int)(background.Height * bgScale)), Color.White);
            }

            spriteBatch.End();

            // Rita ut knapparna
            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnStartGame?.Draw(spriteBatch);
            btnReplayGames?.Draw(spriteBatch);
            btnExitGame?.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
