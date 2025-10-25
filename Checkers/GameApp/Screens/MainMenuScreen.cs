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
    public class MainMenuScreen : Screen
    {
        Texture2D? background;
        float bgScale = 2.0f;
        int bgWidth = 0;

        Button btnStartGame = new Button(new Rectangle(MainGame.WindowWidth / 2 - (250 / 2), MainGame.WindowHeight / 2 - 50, 250, 80), "Start Game");
        Button btnReplayGames= new Button(new Rectangle(MainGame.WindowWidth / 2 - (250 / 2), MainGame.WindowHeight / 2 - 50 + 100, 250, 80), "Replay Games");
        Button btnExitGame = new Button(new Rectangle(MainGame.WindowWidth - 160, MainGame.WindowHeight - 70, 140, 50), "Exit Game");

        public MainMenuScreen()
            : base()
        {
            btnStartGame.Clicked += BtnStartGame_Clicked;
            btnExitGame.Clicked += BtnExitGame_Clicked;
            btnReplayGames.Clicked += BtnReplayGames_Clicked;
        }

        private void BtnReplayGames_Clicked(object? sender, EventArgs e)
        {
            ScreenManager.ChangeScreen(ScreenID.Replay);
        }

        private void BtnExitGame_Clicked(object? sender, EventArgs e)
        {
            MainGame.ExitGame = true;
        }

        private void BtnStartGame_Clicked(object? sender, EventArgs e)
        {
            ScreenManager.ChangeScreen(ScreenID.Game);
        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Checkers");
            bgWidth = (int)(background.Width * bgScale);
            btnStartGame.LoadContent(content);
            btnExitGame.LoadContent(content);
            btnReplayGames.LoadContent(content);
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            btnStartGame.Update(gameTime);
            btnReplayGames.Update(gameTime);
            btnExitGame.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap);

            // Här ritas allt ut
            if (background != null)
            {
                spriteBatch.Draw(background, new Rectangle(MainGame.WindowWidth / 2 - bgWidth / 2, 0, bgWidth, (int)(background.Height * bgScale)), Color.White);
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnStartGame.Draw(spriteBatch);
            btnReplayGames.Draw(spriteBatch);
            btnExitGame.Draw(spriteBatch);

            // Här slutar ritningen

            spriteBatch.End();
        }
    }
}
