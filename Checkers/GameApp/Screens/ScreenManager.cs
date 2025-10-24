using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Checkers.GameApp.Screens
{
    public static class ScreenManager
    {
        private static Screen _currentScreen;
        private static ContentManager _content;
        private static SpriteBatch _spriteBatch;

        private static Dictionary<ScreenID, Screen> _screens = new Dictionary<ScreenID, Screen>();

        public static Screen CurrentScreen
        {
            get 
            { 
                return _currentScreen; 
            }
        }

        public static void Initialize(SpriteBatch spriteBatch, ContentManager content)
        {
            _content = content;
            _spriteBatch = spriteBatch;

            _screens.Add(ScreenID.MainMenu, new MainMenuScreen());
            _screens.Add(ScreenID.Game, new GameScreen());
            _screens.Add(ScreenID.Replay, new ReplayScreen());
        }

        public static void ChangeScreen(ScreenID screenID)
        {
            _currentScreen?.UnloadContent();
            _currentScreen = _screens[screenID];
            _currentScreen.LoadContent(_content);
        }

        public static void Update(GameTime gameTime)
        {
            _currentScreen?.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            _currentScreen?.Draw(_spriteBatch, gameTime);
        }
    }
}
