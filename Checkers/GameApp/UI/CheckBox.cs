using Checkers.GameApp.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Checkers.GameApp.UI
{
    ///<summary>
    /// CheckBox UI komponent
    /// </summary>
    internal class CheckBox : WindowComponent
    {
        const int CHECKBOX_SIZE = 26;
        public event EventHandler? CheckedChanged;

        private Texture2D? uiTexture;
        private Texture2D? uiHoverTexture;
        private Texture2D? uiPressedTexture;
        private Texture2D? activeTexture;
        private Texture2D? markTexture;

        private Label lblText;
        public bool Checked { get; private set; } = false;
        public Color FontColor
        {
            get
            {
                return lblText.FontColor;
            }
            set
            {
                lblText.FontColor = value;
            }
        }

        public CheckBox(Rectangle windowRectangle)
            : base(windowRectangle)
        {
            lblText = new Label(new Rectangle(windowRectangle.X + CHECKBOX_SIZE, windowRectangle.Y, windowRectangle.Width - CHECKBOX_SIZE, windowRectangle.Height));
            Clicked += CheckBox_Clicked;
            FontColor = Color.White;
        }

        public CheckBox(Rectangle windowRectangle, string text)
            : this(windowRectangle)
        {
            lblText.Text = text;
        }

        private void CheckBox_Clicked(object? sender, EventArgs e)
        {
            Checked = !Checked;
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            uiTexture = content.Load<Texture2D>("UINormal");
            uiHoverTexture = content.Load<Texture2D>("UIHover");
            uiPressedTexture = content.Load<Texture2D>("UIDown");
            markTexture = content.Load<Texture2D>("Mark");
            activeTexture = uiTexture;

            lblText.LoadContent(content);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            lblText.UnloadContent();
            uiTexture = null;
            uiHoverTexture = null;
            uiPressedTexture = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsVisible || !Enabled)
            {
                return;
            }

            // Sätt default textur om ingen är satt
            if (activeTexture == null)
            {
                activeTexture = uiTexture;
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
                activeTexture = uiPressedTexture;
            }
            else if (IsMouseOver)
            {
                activeTexture = uiHoverTexture;
            }
            else
            {
                activeTexture = uiTexture;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible || !Enabled)
            {
                return;
            }

            // Kolla det finns en aktiv texture
            if (activeTexture == null)
            {
                // Sätt default texture och hoppa över detta drawcall
                activeTexture = uiTexture;
                return;
            }

            base.Draw(spriteBatch);

            // Rita Checkboxen
            int currX = WindowRectangle.X;
            int currY = WindowRectangle.Y;

            // Use fixed checkbox dimensions
            int boxWidth = CHECKBOX_SIZE;
            int boxHeight = CHECKBOX_SIZE;

            if (Enabled)
            {
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY, 6, 6), new Rectangle(0, 0, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY, boxWidth - 12, 6), new Rectangle(6, 0, 1, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + boxWidth - 6, currY, 6, 6), new Rectangle(activeTexture.Width - 6, 0, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + 6, 6, boxHeight - 12), new Rectangle(0, 6, 6, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + 6, boxWidth - 12, boxHeight - 12), new Rectangle(6, 6, 1, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + boxWidth - 6, currY + 6, 6, boxHeight - 12), new Rectangle(activeTexture.Width - 6, 6, 6, 1), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + boxHeight - 6, 6, 6), new Rectangle(0, activeTexture.Height - 6, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + boxHeight - 6, boxWidth - 12, 6), new Rectangle(6, activeTexture.Height - 6, 1, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + boxWidth - 6, currY + boxHeight - 6, 6, 6), new Rectangle(activeTexture.Width - 6, activeTexture.Height - 6, 6, 6), EnabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY, 6, 6), new Rectangle(0, 0, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY, boxWidth - 12, 6), new Rectangle(6, 0, 1, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + boxWidth - 6, currY, 6, 6), new Rectangle(activeTexture.Width - 6, 0, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + 6, 6, boxHeight - 12), new Rectangle(0, 6, 6, 1), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + 6, boxWidth - 12, boxHeight - 12), new Rectangle(6, 6, 1, 1), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + boxWidth - 6, currY + 6, 6, boxHeight - 12), new Rectangle(activeTexture.Width - 6, 6, 6, 1), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX, currY + boxHeight - 6, 6, 6), new Rectangle(0, activeTexture.Height - 6, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + 6, currY + boxHeight - 6, boxWidth - 12, 6), new Rectangle(6, activeTexture.Height - 6, 1, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
                spriteBatch.Draw(activeTexture, new Rectangle(currX + boxWidth - 6, currY + boxHeight - 6, 6, 6), new Rectangle(activeTexture.Width - 6, activeTexture.Height - 6, 6, 6), DisabledColor, 0f, Vector2.Zero, SpriteEffects.None, 0.0f);
            }

            if(Checked)
            {
                spriteBatch.Draw(markTexture, new Rectangle(currX, currY, boxWidth, boxHeight), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            }

            lblText.Draw(spriteBatch);
        }
    }
}
