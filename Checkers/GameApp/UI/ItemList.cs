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
        private Texture2D buttonTexture;
        private SpriteFont itemFont;
        private Texture2D activeTexture;

        public List<string> Items { get; private set; } = new List<string>();
        public int SelectedIndex { get; private set; } = -1;

        public Color FontColor { get; set; } = Color.Black;

        public Color HoverBackgroundColor { get; set; } = new Color(100, 100, 100, 255);
        public Color HoverFontColor { get; set; } = Color.White;

        private int hoveredIndex = -1;

        private int _scrollOffset = 0;
        private int _scrollStep = 1;
        private const int _borderSize = 6;


        public ItemList(Rectangle windowRectangle)
            : base(windowRectangle)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            itemFont = content.Load<SpriteFont>("Font14");
            buttonTexture = content.Load<Texture2D>("UINormal");
            Items.Add("Item 1");
            Items.Add("Item 2");
            Items.Add("Item 3");
            Items.Add("Item 4");
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
                    int itemTop = itemsStartY + i * itemHeight - _scrollOffset;
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

            int wheelDelta = (int)MouseHelper.LastMouseScrollWheelValue();
            if (IsMouseOver && wheelDelta != 0)
            {
                _scrollOffset -= wheelDelta * _scrollStep;

                int totalContentHeight = itemsStartY + Items.Count * itemHeight + 5;
                int clipHeight = Math.Max(0, WindowRectangle.Height - _borderSize * 2);
                int maxScroll = Math.Max(0, totalContentHeight - clipHeight);

                _scrollOffset = Math.Clamp(_scrollOffset, 0, maxScroll);
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

            int clipX = currX + 6;
            int clipY = currY + 6;
            int clipW = Math.Max(0, WindowRectangle.Width - 12);
            int clipH = Math.Max(0, WindowRectangle.Height - 12);
            var clipRect = new Rectangle(clipX, clipY, clipW, clipH);

            if (clipW <= 0 || clipH <= 0)
            {
                return;
            }

            spriteBatch.End();

            var gd = spriteBatch.GraphicsDevice;
            var prevScissor = gd.ScissorRectangle;

            gd.ScissorRectangle = clipRect;
            using (var rasterizer = new RasterizerState() { ScissorTestEnable = true })
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizer);

                float lHeight = itemFont.MeasureString("TEST").Y + 2f;
                int itemHeight = (int)lHeight;
                int itemsStartY = 5;
                for (int i = 0; i < Items.Count; i++)
                {
                    Vector2 textSize = itemFont.MeasureString(Items[i]);
                    int itemY = currY + itemsStartY + i * itemHeight - _scrollOffset;
                    Rectangle itemBgRect = new Rectangle(currX + 6, itemY, WindowRectangle.Width - 12, itemHeight);
                    Rectangle srcRect = new Rectangle(10, 10, 1, 1);

                    if (i == hoveredIndex)
                    {
                        spriteBatch.Draw(activeTexture, itemBgRect, srcRect, HoverBackgroundColor);
                        spriteBatch.DrawString(itemFont, Items[i], new Vector2(itemBgRect.X + 4, itemBgRect.Y + (itemHeight - textSize.Y) / 2), HoverFontColor);
                    }
                    else
                    {
                        spriteBatch.DrawString(itemFont, Items[i], new Vector2(currX + 10, currY + itemsStartY + i * itemHeight - _scrollOffset + (itemHeight - textSize.Y) / 2), FontColor);
                    }
                }

                spriteBatch.End();
            }

            gd.ScissorRectangle = prevScissor;
            spriteBatch.Begin(SpriteSortMode.Deferred);
        }
    }
}
