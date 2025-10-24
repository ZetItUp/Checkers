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

        

        public Color EnabledColor { get; set; } = Color.White;
        public Color DisabledColor { get; set; } = Color.CadetBlue;
        public bool Enabled { get; set; } = true;

        public ComboBox(Rectangle windowRectangle)
            : base(windowRectangle)
        {

        }
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            buttonFont = content.Load<SpriteFont>("Font12");
            buttonTexture = content.Load<Texture2D>("UINormal");
            buttonHoverTexture = content.Load<Texture2D>("UIHover");
            buttonPressedTexture = content.Load<Texture2D>("UIDown");
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
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);


        }
    }
}
