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
        public Rectangle WindowRectangle { get; set; } = new Rectangle(0, 0, 0, 0);
        public bool IsVisible { get; set; } = true;
        public bool IsMouseOver { get; protected set; } = false;

        public WindowComponent(Rectangle windowRectangle)
        {
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
