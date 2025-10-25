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
        // Event som triggas när valt item ändras
        public event EventHandler? SelectedItemChanged;

        // Texturer och font 
        private Texture2D? buttonTexture;
        private Texture2D? buttonHoverTexture;
        private Texture2D? buttonPressedTexture;
        private Texture2D? activeTexture;
        private SpriteFont? buttonFont;

        // Lista med items och state för att visa listan
        bool showList = false;
        ItemList? itemList;
        public string SelectedItemText = string.Empty;

        // Färg för texten
        public Color FontColor { get; set; } = Color.Black;

        public ComboBox(Rectangle windowRectangle)
            : base(windowRectangle)
        {
            // Initialisera itemList under comboboxen
            itemList = new ItemList(new Rectangle(windowRectangle.X, windowRectangle.Y + windowRectangle.Height, windowRectangle.Width, 100));
            showList = false;
        }

        public override void LoadContent(ContentManager content)
        {
            // Ladda in texturer och font
            base.LoadContent(content);
            buttonFont = content.Load<SpriteFont>("Font14");
            buttonTexture = content.Load<Texture2D>("UINormal");
            buttonHoverTexture = content.Load<Texture2D>("UIHover");
            buttonPressedTexture = content.Load<Texture2D>("UIDown");
            itemList?.LoadContent(content);
            activeTexture = buttonTexture;
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

            // Hantera Vilken textur som ska användas beroende på musens state
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

            // Om musen är över comboboxen och vänstra knappen släpptes, toggla visningen av listan
            if (IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left))
            {
                showList = !showList;
            }

            // Uppdatera itemList om den ska visas
            if (itemList != null)
            {
                if (showList)
                {
                    // Uppdatera bara itemlist om den är synlig
                    itemList.Update(gameTime);

                    // Om musen är över itemList och vänstra knappen släpptes, välj det item som musen är över
                    if (itemList.IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left) && itemList.SelectedIndex >= 0)
                    {
                        showList = false;
                        SelectedItemText = itemList.Items[itemList.SelectedIndex];

                        // Invoke:a eventet för att valt item har ändrats
                        SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                    }
                }

                // Uppdatera synligheten endast om showList har ändrats, för att undvika onödiga uppdateringar och/eller flickering
                if (itemList.IsVisible != showList)
                {
                    itemList.IsVisible = showList;
                }
            }

            // Stäng listan om musen klickar utanför comboboxen och listan
            // Här spelar ordningen roll, dvs den här checken måste köras efter att itemList har uppdaterats
            // annars blir showList felaktigt satt till false direkt efter att den satts till true ovan
            if (!IsMouseOver && MouseHelper.MouseReleased(MouseHelper.MouseButton.Left))
            {
                showList = false;
            }
        }

        /// <summary>
        /// Lägg till ett item i comboboxen
        /// </summary>
        /// <param name="item">Strängen att lägga till</param>
        public void AddItem(string item)
        {
            itemList?.Items.Add(item);
        }

        /// <summary>
        /// Ta bort ett item från comboboxen
        /// </summary>
        /// <param name="item">Strängen att ta bort</param>
        public void RemoveItem(string item)
        {
            itemList?.Items.Remove(item);
        }

        /// <summary>
        /// Cleara alla items från comboboxen
        /// </summary>
        public void ClearItems()
        {
            itemList?.Items.Clear();
            SelectedItemText = string.Empty;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            itemList?.UnloadContent();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if(activeTexture == null)
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

            // Se till att itemList och buttonFont inte är null innan vi ritar dem
            if (itemList != null && buttonFont != null)
            {
                // Kolla om listan ska ritas
                if (showList)
                {
                    itemList.Draw(spriteBatch);
                }

                // Rita den valda texten ovanför itemList, då allting ritas i ordning 
                if (!string.IsNullOrEmpty(SelectedItemText))
                {
                    // Räkna ut textens position för att centrera den i comboboxen
                    Vector2 textSize = buttonFont.MeasureString(SelectedItemText);
                    Vector2 textPosition = new Vector2(currX + (WindowRectangle.Width - textSize.X) / 2, currY + (WindowRectangle.Height - textSize.Y) / 2);
                    spriteBatch.DrawString(buttonFont, SelectedItemText, textPosition, FontColor);
                }
            }
        }
    }
}
