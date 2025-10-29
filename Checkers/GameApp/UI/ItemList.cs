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
    /// ItemList Komponent
    /// </summary>
    internal class ItemList : WindowComponent
    {
        // Texturer och font
        private Texture2D? buttonTexture;
        private Texture2D? activeTexture;
        private SpriteFont? itemFont;

        // Lista med items som ska visas
        public List<string> Items { get; private set; } = new List<string>();
        // Index för det valda itemet
        public int SelectedIndex { get; private set; } = -1;
        // Färger
        public Color FontColor { get; set; } = Color.Black;

        public Color HoverBackgroundColor { get; set; } = new Color(66, 79, 153);
        public Color HoverFontColor { get; set; } = Color.White;

        // Beteende variabler för itemlista
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
            // Ladda in texturer och font
            base.LoadContent(content);
            itemFont = content.Load<SpriteFont>("Font14");
            buttonTexture = content.Load<Texture2D>("UINormal");
            activeTexture = buttonTexture;
        }

        public override void UnloadContent()
        {
            // Återställ alla variabler och töm listan
            base.UnloadContent();
            Items.Clear();
            SelectedIndex = -1;
            hoveredIndex = -1;
            _scrollOffset = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Felhantering för komponentens synlighet och om den är aktiverad
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

            if(itemFont == null)
            {
                return;
            }

            // Kolla musposition relativt till komponenten
            var mouse = MouseHelper.MousePosition();
            int localX = (int)mouse.X - WindowRectangle.X;
            int localY = (int)mouse.Y - WindowRectangle.Y;

            // Ränkna höjden för varje item baserat på fontstorleken, lägg till offset 
            float lineHeight = itemFont.MeasureString("TEST").Y + 2f;
            int itemHeight = (int)lineHeight;
            int itemsStartY = 5;

            // Kolla om musen är inom komponentens rektangel
            if (localX >= 0 && localY >= 0 && localX <= WindowRectangle.Width && localY <= WindowRectangle.Height)
            {
                // Loopa igenom items för att se om musen är över något item
                for (int i = 0; i < Items.Count; i++)
                {
                    // Räkna ut itemets rektangel
                    int itemTop = itemsStartY + i * itemHeight - _scrollOffset;
                    Rectangle itemRectLocal = new Rectangle(10, itemTop, WindowRectangle.Width - 20, itemHeight);

                    // Kolla om musen är över itemets rektangel
                    if (localX >= itemRectLocal.X && localX <= itemRectLocal.X + itemRectLocal.Width
                        && localY >= itemRectLocal.Y && localY <= itemRectLocal.Y + itemRectLocal.Height)
                    {
                        // Sätt hoveredIndex till nuvarande item index
                        hoveredIndex = i;

                        // Kolla om vänstra musknappen släpptes för att välja item
                        if (MouseHelper.MouseReleased(MouseButton.Left))
                        {
                            SelectedIndex = i;
                        }

                        break;
                    }
                }
            }

            // Hantera mousewheel för att scrolla igenom listan
            int wheelDelta = (int)MouseHelper.LastMouseScrollWheelValue();

            // Kolla om musen är över komponenten och om det finns någon scroll-rörelse
            if (IsMouseOver && wheelDelta != 0)
            {
                // Uppdatera scroll offset, multiplicera wheelDelta med scrollSteg 
                _scrollOffset -= wheelDelta * _scrollStep;

                // Begränsa scrollOffset så att den inte går utanför innehållets gränser
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

            // Rita komponenten med olika färger beroende på om den är aktiv eller inte
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

            // Klipp bort överflödigt innehåll utanför komponentens rektangel
            // Definiera klipprektangeln
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

            // Sätt scissor rektangeln för att begränsa ritningen
            gd.ScissorRectangle = clipRect;
            using (var rasterizer = new RasterizerState() { ScissorTestEnable = true })
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizer);

                // Beräkna höjden för ett item baserat på fontstorleken
                float lHeight = itemFont.MeasureString("TEST").Y + 2f;
                int itemHeight = (int)lHeight;
                int itemsStartY = 5;

                // Gå igenom all items
                for (int i = 0; i < Items.Count; i++)
                {
                    // Mät textstorleken för nuvarande item
                    Vector2 textSize = itemFont.MeasureString(Items[i]);
                    // Räkna ut itemets position 
                    int itemY = currY + itemsStartY + i * itemHeight - _scrollOffset;
                    // Räkna ut rektanglar för item bakgrund och källa
                    Rectangle itemBgRect = new Rectangle(currX + 6, itemY, WindowRectangle.Width - 12, itemHeight);
                    Rectangle srcRect = new Rectangle(10, 10, 1, 1);

                    // Om itemet är hoverat
                    if (hoveredIndex == i)
                    {
                        // Rita bakgrunden med hoverbgcolor och rita texten med hoverfont
                        spriteBatch.Draw(activeTexture, itemBgRect, srcRect, HoverBackgroundColor);
                        spriteBatch.DrawString(itemFont, Items[i], new Vector2(itemBgRect.X + 4, itemBgRect.Y + (itemHeight - textSize.Y) / 2), HoverFontColor);
                    }
                    else
                    {
                        // Rita bara ut texten med vanlig fontcolor, ingen bakgrund
                        spriteBatch.DrawString(itemFont, Items[i], new Vector2(currX + 10, currY + itemsStartY + i * itemHeight - _scrollOffset + (itemHeight - textSize.Y) / 2), FontColor);
                    }
                }

                spriteBatch.End();
            }

            // Återställ tidigare scissor rektangel
            gd.ScissorRectangle = prevScissor;

            // Återuppta spritebatchen
            spriteBatch.Begin(SpriteSortMode.Deferred);
        }
    }
}
