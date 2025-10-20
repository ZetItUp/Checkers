using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens
{
    public class MainMenuScreen : Screen
    {
        Texture2D background;
        public MainMenuScreen()
            : base()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Checkers");
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Vector2(Checkers.WindowWidth / 2 - background.Width / 2, 50), Color.White);

            spriteBatch.End();
        }
    }
}
