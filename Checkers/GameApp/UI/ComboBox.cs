using Checkers.GameApp.Helpers;
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
    internal class ComboBox : WindowComponent
    {
        public event EventHandler? SelectedItemChanged;

        Texture2D buttonTexture;
        Texture2D buttonHoverTexture;
        Texture2D buttonPressedTexture;
        SpriteFont buttonFont;

        Texture2D activeTexture;

        bool showList = false;
        ItemList itemList;
        public string SelectedItemText = string.Empty;

        public Color FontColor { get; set; } = Color.Black;

        public ComboBox(Rectangle windowRectangle)
            : base(windowRectangle)
        {
            itemList = new ItemList(new Rectangle(windowRectangle.X, windowRectangle.Y + windowRectangle.Height, windowRectangle.Width, 100));
            showList = false;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            buttonFont = content.Load<SpriteFont>("Font14");
            buttonTexture = content.Load<Texture2D>("UINormal");
            buttonHoverTexture = content.Load<Texture2D>("UIHover");
            buttonPressedTexture = content.Load<Texture2D>("UIDown");
            itemList.LoadContent(content);
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
                showList = !showList;
            }
            

            if (showList)
            {
                itemList.Update(gameTime);
                if (itemList.IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left) && itemList.SelectedIndex >= 0)
                {
                    showList = false;
                    SelectedItemText = itemList.Items[itemList.SelectedIndex];
                    SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            if (itemList.IsVisible != showList)
            {
                itemList.IsVisible = showList;
            }

            if (!IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left))
            {
                showList = false;
            }
        }

        public void AddItem(string item)
        {
            itemList.Items.Add(item);
        }

        public void RemoveItem(string item)
        {
            itemList.Items.Remove(item);
        }

        public void ClearItems()
        {
            itemList.Items.Clear();
            SelectedItemText = string.Empty;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(activeTexture == null)
            {
                activeTexture = buttonTexture;
            }

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

            if (showList)
            {
                itemList.Draw(spriteBatch);
            }

            // Draw selected item text
            if (!string.IsNullOrEmpty(SelectedItemText))
            {
                Vector2 textSize = buttonFont.MeasureString(SelectedItemText);
                Vector2 textPosition = new Vector2(currX + (WindowRectangle.Width - textSize.X) / 2, currY + (WindowRectangle.Height - textSize.Y) / 2);
                spriteBatch.DrawString(buttonFont, SelectedItemText, textPosition, FontColor);
            }
        }
    }
}
