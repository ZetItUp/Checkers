using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens
{
    // Tom basklass för olika screens i spelet
    // Denna klass skulle kunna vara abstract, men vi var osäkra på om vi skulle behöva en instans av Screen direkt någon gång under testning
    public class Screen
    {
        public Screen()
        {

        }

        public virtual void LoadContent(ContentManager content)
        {

        }
        public virtual void UnloadContent()
        {

        }
        public virtual void Update(GameTime gameTime)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }
    }
}
