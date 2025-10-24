using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.GameApp.Helpers;
using Checkers.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Checkers.GameApp.UI
{
    internal class ItemList : WindowComponent
    {
        Texture2D buttonTexture;
        Texture2D buttonHoverTexture;
        Texture2D buttonPressedTexture;
        SpriteFont buttonFont;

        Texture2D activeTexture;

        public List<string> Items { get; private set; } = new List<string>();
        public int SelectedIndex { get; private set; } = -1;

        public ItemList(Rectangle windowRectangle) 
            : base(windowRectangle)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            buttonFont = content.Load<SpriteFont>("Font12");
            buttonTexture = content.Load<Texture2D>("UINormal");
            buttonHoverTexture = content.Load<Texture2D>("UIHover");
            buttonPressedTexture = content.Load<Texture2D>("UIDown");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Enabled)
            {
                if (activeTexture == null)
                {
                    activeTexture = buttonTexture;
                }

                return;
            }

            // Kolla om vänstra musknappen är nedtryckt
            if (IsMouseOver && MouseHelper.MouseDown(MouseHelper.MouseButton.Left))
            {
                activeTexture = buttonPressedTexture;
            }
            else if (IsMouseOver)
            {
                activeTexture = buttonHoverTexture;
            }
            else
            {
                activeTexture = buttonTexture;
            }

            if (IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left))
            {
                
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(!IsVisible)
            {
                return;
            }
            base.Draw(spriteBatch);


        }
    }
}
