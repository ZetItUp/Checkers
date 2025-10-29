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
    internal class Label : WindowComponent
    {
        // Label Font
        SpriteFont? buttonFont;
        // Font Color
        public Color FontColor = Color.Black;
        // Text som skrivs ut
        public string Text { get; set; } = string.Empty;

        public Label(Rectangle windowRectangle)
            : base(windowRectangle)
        {

        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);

            buttonFont = content.Load<SpriteFont>("Font14");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Skippa drawcall om fonten inte är laddad
            if(buttonFont == null)
            {
                return;
            }

            // Kolla om knappen är synlig
            if (!IsVisible)
            {
                return;
            }

            base.Draw(spriteBatch);

            int currX = WindowRectangle.X;
            int currY = WindowRectangle.Y;

            // Klipp bort överflödigt innehåll utanför komponentens rektangel
            // Definiera klipprektangeln
            int clipX = WindowRectangle.X;
            int clipY = WindowRectangle.Y;
            int clipW = Math.Max(0, WindowRectangle.Width);
            int clipH = Math.Max(0, WindowRectangle.Height);
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

                // Mät textstorleken för nuvarande item
                Vector2 textSize = buttonFont.MeasureString(Text);
                spriteBatch.DrawString(buttonFont, Text, new Vector2(currX + 5, currY), FontColor);

                spriteBatch.End();
            }

            // Återställ tidigare scissor rektangel
            gd.ScissorRectangle = prevScissor;

            // Återuppta spritebatchen
            spriteBatch.Begin(SpriteSortMode.Deferred);
        }
    }
}
