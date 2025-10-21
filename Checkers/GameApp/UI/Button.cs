using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.GameApp.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Checkers.UI
{
    internal class Button : WindowComponent
    {
        public event EventHandler? Clicked;

        Texture2D buttonTexture;
        Texture2D buttonHoverTexture;
        Texture2D buttonPressedTexture;
        SpriteFont buttonFont;

        public string Text { get; set; } = "Button";
        
        public Button(Rectangle buttonRectangle)
            : base(buttonRectangle)
        {
            
        }

        public Button(Rectangle buttonRectangle, string text)
            : base(buttonRectangle)

        {
            Text = text;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            buttonFont = content.Load<SpriteFont>("Font12");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Kolla om vänstra musknappen är nedtryckt
            if (IsMouseOver && MouseHelper.MouseDown(MouseHelper.MouseButton.Left))
            {
                // Anropa Clicked-händelsen
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(IsMouseOver && MouseHelper.MouseDown(MouseHelper.MouseButton.Left))
            {
                // Rita hover texture
                spriteBatch.Draw(buttonPressedTexture, WindowRectangle, Color.White);
            }
            else if(IsMouseOver)
            {
                // Rita pressed texture
                spriteBatch.Draw(buttonHoverTexture, WindowRectangle, Color.White);
            }
            else
            {
                // Rita vanlig texture
                spriteBatch.Draw(buttonTexture, WindowRectangle, Color.White);
            }

            // Rita texten centrerad på knappen
            Vector2 textSize = buttonFont.MeasureString(Text);
            Vector2 textPosition = new Vector2(
                WindowRectangle.X + (WindowRectangle.Width - textSize.X) / 2,
                WindowRectangle.Y + (WindowRectangle.Height - textSize.Y) / 2
            );
            spriteBatch.DrawString(buttonFont, Text, textPosition, Color.Black);
        }
    }
}
