using Checkers.GameApp.Screens.Interfaces;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens
{
    public sealed class AppContext : IAppContext
    {
        public AppContext(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice graphics) 
        { 
            SpriteBatch = spriteBatch;
            Content = content;
            GraphicsDevice = graphics;
        }

        public SpriteBatch SpriteBatch { get; private set; }

        public ContentManager Content { get; private set; }

        public GraphicsDevice GraphicsDevice { get; private set; }
    }
}
