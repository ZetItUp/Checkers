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
        float bgScale = 2.0f;
        int bgWidth = 0;

        public MainMenuScreen()
            : base()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            background = content.Load<Texture2D>("Checkers");
            bgWidth = (int)(background.Width * bgScale);
        }

        public override void UnloadContent()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap);

            // Här ritas allt ut
            spriteBatch.Draw(background, new Rectangle(MainGame.WindowWidth / 2 - bgWidth / 2, 0, bgWidth, (int)(background.Height * bgScale)), Color.White);


            // Här slutar ritningen
            spriteBatch.End();
        }
    }
}
