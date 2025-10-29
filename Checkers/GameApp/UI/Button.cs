using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.GameApp.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Checkers.GameApp.UI
{
    /// <summary>
    /// Knapp Komponent
    /// </summary>
    internal class Button : WindowComponent
    {
        // Event som triggas när knappen klickas
        public event EventHandler? Clicked;

        // Textur och font 
        Texture2D? buttonTexture;
        Texture2D? buttonHoverTexture;
        Texture2D? buttonPressedTexture;
        Texture2D? activeTexture;
        SpriteFont? buttonFont;

        // Text som visas på knappen
        public string Text { get; set; } = "Button";

        /// <summary>
        /// Skapa en Button komponent med default text
        /// </summary>
        /// <param name="buttonRectangle">Button Size</param>
        public Button(Rectangle buttonRectangle)
            : base(buttonRectangle)
        {

        }

        /// <summary>
        /// Skapa en Button komponent med specific text
        /// </summary>
        /// <param name="buttonRectangle">Button Size</param>
        /// <param name="text">Button Text</param>
        public Button(Rectangle buttonRectangle, string text)
            : base(buttonRectangle)
        {
            Text = text;
        }

        public override void LoadContent(ContentManager content)
        {
            // Ladda in texturer och font
            base.LoadContent(content);
            buttonFont = content.Load<SpriteFont>("Font14");
            buttonTexture = content.Load<Texture2D>("UINormal");
            buttonHoverTexture = content.Load<Texture2D>("UIHover");
            buttonPressedTexture = content.Load<Texture2D>("UIDown");
            activeTexture = buttonTexture;
        }

        public override void UnloadContent()
        {
            // Töm resurser
            base.UnloadContent();
            buttonFont = null;
            buttonTexture = null;
            buttonHoverTexture = null;
            buttonPressedTexture = null;
            activeTexture = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Sätt default textur om ingen är satt
            if (activeTexture == null)
            {
                activeTexture = buttonTexture;
            }

            // Hantera inte input om knappen är inaktiverad
            if (!Enabled)
            {
                return;
            }

            // Hantera Vilken textur som ska användas beroende på musens state
            // Kolla om vänstra musknappen är nedtryckt
            if (IsMouseOver && MouseHelper.MouseDown(MouseButton.Left))
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

            // Kolla om knappen har klickats
            if (IsMouseOver && MouseHelper.MouseReleased(MouseButton.Left))
            {
                // Invoke:a Clicked eventet
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Kolla om knappen är synlig
            if (!IsVisible)
            {
                return;
            }

            // Kolla det finns en aktiv texture
            if (activeTexture == null)
            {
                // Sätt default texture och hoppa över detta drawcall
                activeTexture = buttonTexture;
                return;
            }

            base.Draw(spriteBatch);

            // Rita knappen med 9-slice scaling
            // Dvs dela upp texturen i 9 delar och skala den korrekt
            int currX = WindowRectangle.X;
            int currY = WindowRectangle.Y;
            if (Enabled)
            {
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY, 6, 6), new Rectangle(0, 0, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY, WindowRectangle.Width - 12, 6), new Rectangle(6, 0, 1, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY, 6, 6), new Rectangle(activeTexture.Width - 6, 0, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(0, 6, 6, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + 6, WindowRectangle.Width - 12, WindowRectangle.Height - 12), new Rectangle(6, 6, 1, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(activeTexture.Width - 6, 6, 6, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(0, activeTexture.Height - 6, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + WindowRectangle.Height - 6, WindowRectangle.Width - 12, 6), new Rectangle(6, activeTexture.Height - 6, 1, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(activeTexture.Width - 6, activeTexture.Height - 6, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY, 6, 6), new Rectangle(0, 0, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY, WindowRectangle.Width - 12, 6), new Rectangle(6, 0, 1, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY, 6, 6), new Rectangle(activeTexture.Width - 6, 0, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(0, 6, 6, 1), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + 6, WindowRectangle.Width - 12, WindowRectangle.Height - 12), new Rectangle(6, 6, 1, 1), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + 6, 6, WindowRectangle.Height - 12), new Rectangle(activeTexture.Width - 6, 6, 6, 1), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(0, activeTexture.Height - 6, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + WindowRectangle.Height - 6, WindowRectangle.Width - 12, 6), new Rectangle(6, activeTexture.Height - 6, 1, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + WindowRectangle.Width - 6, currY + WindowRectangle.Height - 6, 6, 6), new Rectangle(activeTexture.Width - 6, activeTexture.Height - 6, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap);
            // Rita texten centrerad på knappen
            if (buttonFont != null)
            {
                // Mät hur stor texten är så vi kan centrera den
                Vector2 textSize = buttonFont.MeasureString(Text);
                Vector2 textPosition = new Vector2(
                    WindowRectangle.X + (WindowRectangle.Width - textSize.X) / 2,
                    WindowRectangle.Y + (WindowRectangle.Height - textSize.Y) / 2
                );

                // Rita texten
                spriteBatch.DrawString(buttonFont, Text, textPosition, Color.Black, 0f, Vector2.Zero, new Vector2(1f, 1.0f), SpriteEffects.None, 1f);
            }
        }
    }
}
