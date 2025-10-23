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

        Texture2D activeTexture;

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
            buttonFont = content.Load<SpriteFont>("Font14");
            buttonTexture = content.Load<Texture2D>("UINormal");
            buttonHoverTexture = content.Load<Texture2D>("UIHover");
            buttonPressedTexture = content.Load<Texture2D>("UIDown");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

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

            if(IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left))
            {
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(buttonPressedTexture == null || buttonHoverTexture == null || buttonTexture == null)
            {
                return;    
            }

            if(activeTexture == null)
            {
                return;
            }

            int currX = WindowRectangle.X;
            int currY = WindowRectangle.Y;

            spriteBatch.Draw(activeTexture, new Rectangle(currX, currY, 6, 6), new Rectangle(0, 0, 6, 6), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY, WindowRectangle.Width - 12, 6), new Rectangle(6, 0, 1, 6), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY, 6, 6), new Rectangle(activeTexture.Width - 6, 0, 6, 6), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(0, 6, 6, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + 6, WindowRectangle.Width - 12, WindowRectangle.Height - 12), new Rectangle(6, 6, 1, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(activeTexture.Width - 6, 6, 6, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(0, activeTexture.Height - 6, 6, 6), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + WindowRectangle.Height - 6, WindowRectangle.Width - 12, 6), new Rectangle(6, activeTexture.Height - 6, 1, 6), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(activeTexture.Width - 6, activeTexture.Height - 6, 6, 6), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);


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
