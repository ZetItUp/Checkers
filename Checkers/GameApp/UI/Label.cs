using Checkers.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.UI
{
    internal class Label : WindowComponent
    {
        SpriteFont? buttonFont;

        public Label(Rectangle windowRectangle)
            : base(windowRectangle)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            buttonFont = content.Load<SpriteFont>("Font14");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
