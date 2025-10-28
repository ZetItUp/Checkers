using Checkers.GameApp.Screens.Interfaces;
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
    // Screen Interface
    public interface IScreen
    {
        public void LoadContent(IAppContext appContext);
        public void UnloadContent();
        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime);
    }
}
