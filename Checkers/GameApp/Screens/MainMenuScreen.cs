using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        // Texture för bakgrunden
        Texture2D? background;

        // Skalningsfaktor och bredd för bakgrunden
        float bgScale = 2.0f;
        int bgWidth = 0;

        // Knappar för menyn
        Button btnStartGame = new Button(new Rectangle(MainGame.WindowWidth / 2 - (250 / 2), MainGame.WindowHeight / 2 - 50, 250, 80), "Start Game");
        Button btnReplayGames= new Button(new Rectangle(MainGame.WindowWidth / 2 - (250 / 2), MainGame.WindowHeight / 2 - 50 + 100, 250, 80), "Replay Games");
        Button btnExitGame = new Button(new Rectangle(MainGame.WindowWidth - 160, MainGame.WindowHeight - 70, 140, 50), "Exit Game");

        public MainMenuScreen()
            : base()
        {
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
            ScreenManager.ChangeScreen(ScreenID.Replay);
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
            ScreenManager.ChangeScreen(ScreenID.Game);
        }

        public void LoadContent(ContentManager content)
        {
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

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
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
