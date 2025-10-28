using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens.Interfaces
{
    // Application Context Interface, håller reda på SpriteBatch, ContentManager och GraphicsDevice
    public interface IAppContext
    {
        SpriteBatch SpriteBatch { get; }
        ContentManager Content { get; }
        GraphicsDevice GraphicsDevice { get; }
    }
}
