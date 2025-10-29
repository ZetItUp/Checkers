using Checkers.GameApp.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.UI
{
    /// <summary>
    /// Basklass för UI Komponenter
    /// </summary>
    internal class WindowComponent
    {
        // Event som triggas när knappen klickas
        public event EventHandler? Clicked;

        // Värden för minimistorlek på fönstret
        private const int MIN_WINDOW_WIDTH = 20;
        private const int MIN_WINDOW_HEIGHT = 20;

        // Rectangle som definierar fönstrets position och storlek
        public Rectangle WindowRectangle { get; set; } = new Rectangle(0, 0, MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);
        // Är componenten synlig?
        public bool IsVisible { get; set; } = true;
        // Är musen över componenten?
        public bool IsMouseOver { get; protected set; } = false;
        // Är componenten aktiv?
        public bool Enabled { get; set; } = true;
        // Färger för enabled och disabled state
        public Color EnabledColor { get; set; } = Color.White;
        public Color DisabledColor { get; set; } = new Color(0x56, 0xB4, 0xE9, 120);

        public WindowComponent(Rectangle windowRectangle)
        {
            // Se till att windowRectangle inte är mindre än minimistorleken
            if (windowRectangle.Width < MIN_WINDOW_WIDTH)
            {
                windowRectangle.Width = MIN_WINDOW_WIDTH;
            }

            if(windowRectangle.Height < MIN_WINDOW_HEIGHT)
            {
                windowRectangle.Height = MIN_WINDOW_HEIGHT;
            }

            WindowRectangle = windowRectangle;
        }

        public virtual void LoadContent(ContentManager content)
        {

        }

        public virtual void UnloadContent()
        {
            // Återställ variabler till default värden
            IsVisible = true;
            Enabled = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            // Hantera inte input om knappen är inaktiverad
            if (!Enabled || !IsVisible)
            {
                return;
            }

            // Kolla om musen är över componenten
            if (MouseHelper.MouseRectangle().Intersects(WindowRectangle))
            {
                IsMouseOver = true;
            }
            else
            {
                IsMouseOver = false;
            }

            // Kolla om komponenten har klickats på
            if (IsMouseOver && MouseHelper.MouseReleased(MouseButton.Left))
            {
                // Invoke:a Clicked eventet
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
