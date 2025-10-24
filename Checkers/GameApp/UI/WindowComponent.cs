using Checkers.GameApp.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.UI
{
    internal class WindowComponent
    {
        private const int MIN_WINDOW_WIDTH = 20;
        private const int MIN_WINDOW_HEIGHT = 20;

        public Rectangle WindowRectangle { get; set; } = new Rectangle(0, 0, MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
        public bool IsVisible { get; set; } = true;
        public bool IsMouseOver { get; protected set; } = false;
        public bool Enabled { get; set; } = true;

        public WindowComponent(Rectangle windowRectangle)
        {
            // Se till att windowRectangle inte är mindre än minimistorleken
            if (windowRectangle.Width < MIN_WINDOW_WIDTH)
            {
                windowRectangle.Width = MIN_WINDOW_WIDTH;
            }

            if(windowRectangle.Height < MIN_WINDOW_HEIGHT)
            {
                windowRectangle.Height = MIN_WINDOW_HEIGHT;
            }

            WindowRectangle = windowRectangle;
        }

        public virtual void LoadContent(ContentManager content)
        {

        }

        public virtual void Update(GameTime gameTime)
        {

            if(MouseHelper.MouseRectangle().Intersects(WindowRectangle))
            {
                IsMouseOver = true;
            }
            else
            {
                IsMouseOver = false;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
