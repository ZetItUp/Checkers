using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp
{
    // Helper class för grafik
    public class GraphicsHelper
    {
        // Skapa en texture box med en vald storlek och färg
        public static Texture2D CreateTexture(GraphicsDevice gfxDev, int Width, int Height, Color textureColor)
        {
            Texture2D bgText = new Texture2D(gfxDev, Width, Height);
            Color[] bgColor = new Color[Width * Height];

            for (int i = 0; i < bgColor.Length; i++)
            {
                bgColor[i] = new Color(textureColor.R, textureColor.G, textureColor.B, (byte)255);
            }

            bgText.SetData(bgColor);

            return bgText;
        }
    }
}
