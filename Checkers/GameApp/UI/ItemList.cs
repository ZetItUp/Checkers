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
        SpriteFont itemFont;
        Texture2D activeTexture;

        public List<string> Items { get; private set; } = new List<string>();
        public int SelectedIndex { get; private set; } = -1;

        public Color FontColor { get; set; } = Color.Black;

        public Color HoverBackgroundColor { get; set; } = new Color(100, 100, 100, 255);
        public Color HoverFontColor { get; set; } = Color.White;

        int hoveredIndex = -1;

        public ItemList(Rectangle windowRectangle)
            : base(windowRectangle)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            itemFont = content.Load<SpriteFont>("Font12");
            buttonTexture = content.Load<Texture2D>("UINormal");
            Items.Add("Item 1");
            Items.Add("Item 2");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsVisible)
            {
                return;
            }
            
            if (activeTexture == null)
            {
                activeTexture = buttonTexture;
            }

            hoveredIndex = -1;

            if (!Enabled)
            {
                return;
            }

            var mouse = MouseHelper.MousePosition();
            int localX = (int)mouse.X - WindowRectangle.X;
            int localY = (int)mouse.Y - WindowRectangle.Y;

            float lineHeight = itemFont.MeasureString("TEST").Y + 2f;
            int itemHeight = (int)lineHeight;
            int itemsStartY = 5;

            if (localX >= 0 && localY >= 0 && localX <= WindowRectangle.Width && localY <= WindowRectangle.Height)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    int itemTop = itemsStartY + i * itemHeight;
                    Rectangle itemRectLocal = new Rectangle(10, itemTop, WindowRectangle.Width - 20, itemHeight);

                    if (localX >= itemRectLocal.X && localX <= itemRectLocal.X + itemRectLocal.Width
                        && localY >= itemRectLocal.Y && localY <= itemRectLocal.Y + itemRectLocal.Height)
                    {
                        hoveredIndex = i;

                        if (MouseHelper.MouseReleased(MouseHelper.MouseButton.Left))
                        {
                            SelectedIndex = i;
                        }

                        break;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
            {
                return;
            }
            base.Draw(spriteBatch);

            if (activeTexture == null || itemFont == null)
            {
                return;
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

            // Draw items
            float lHeight = itemFont.MeasureString("TEST").Y + 2f;
            int itemHeight = (int)lHeight;
            for (int i = 0; i < Items.Count; i++)
            {
                Vector2 textSize = itemFont.MeasureString(Items[i]);
                Rectangle itemBgRect = new Rectangle(currX + 10, currY + 5 + i * itemHeight, WindowRectangle.Width - 20, itemHeight);
                Rectangle srcRect = new Rectangle(10, 10, 1, 1);

                if (i == hoveredIndex)
                {
                    spriteBatch.Draw(activeTexture, itemBgRect, srcRect, HoverBackgroundColor);
                    spriteBatch.DrawString(itemFont, Items[i], new Vector2(itemBgRect.X + 0, itemBgRect.Y + (itemHeight - textSize.Y) / 2), HoverFontColor);
                }
                else
                {
                    spriteBatch.DrawString(itemFont, Items[i], new Vector2(currX + 10, currY + 5 + i * itemHeight + (itemHeight - textSize.Y) / 2), FontColor);
                }
            }
        }
    }
}
