using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Checkers.GameApp.Screens.Interfaces
{
    ///<summary>
    /// Application Context Interface, håller reda på SpriteBatch, ContentManager och GraphicsDevice
    ///</summary>
    public interface IAppContext
    {
        SpriteBatch SpriteBatch { get; }
        ContentManager Content { get; }
        GraphicsDevice GraphicsDevice { get; }
    }
}
