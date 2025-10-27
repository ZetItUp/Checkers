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
    // Statisk klass för att hantera olika screens i spelet
    public static class ScreenManager
    {
        // Nuvarande aktiva screen
        private static IScreen? _currentScreen;
        // ContentManager för att ladda innehåll
        private static ContentManager? _content;
        // SpriteBatch för att rita grafik
        private static SpriteBatch? _spriteBatch;
        // Dictionary för att lagra olika screens, mappade till deras ScreenID
        private static Dictionary<ScreenID, IScreen> _screens = new Dictionary<ScreenID, IScreen>();

        /// <summary>
        /// Hämta nuvarande aktiva screen
        /// </summary>
        public static IScreen CurrentScreen
        {
            get 
            { 
                return _currentScreen!; 
            }
        }

        /// <summary>
        /// Initialisera ScreenManager med SpriteBatch och ContentManager
        /// Detta måste kallas innan någon annan metod används
        /// </summary>
        /// <param name="spriteBatch">Aktiv SpriteBatch från GraphicsDevice</param>
        /// <param name="content">Aktiv ContentMangaer</param>
        public static void Initialize(SpriteBatch spriteBatch, ContentManager content)
        {
            _content = content;
            _spriteBatch = spriteBatch;
        }

        /// <summary>
        /// Lägg till en screen i ScreenManager
        /// </summary>
        /// <param name="screenID">ScreenID for screen, must be unique</param>
        /// <param name="screen">Screen object</param>
        public static void AddScreen(ScreenID screenID, IScreen screen)
        {
            _screens.Add(screenID, screen);
        }

        /// <summary>
        /// Byt screen till den angivna ScreenID
        /// </summary>
        /// <param name="screenID">ScreenID för den skärm som önskas bytas till</param>
        public static void ChangeScreen(ScreenID screenID)
        {
            // Töm nuvarande screen om den finns
            _currentScreen?.UnloadContent();
            // Sätt aktiv screen till den nya och ladda dess innehåll
            _currentScreen = _screens[screenID];
            _currentScreen.LoadContent(_content!);
        }

        public static void Update(GameTime gameTime)
        {
            // Uppdatera nuvarande screen om den finns
            _currentScreen?.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            // Rita nuvarande screen om den finns
            _currentScreen?.Draw(_spriteBatch!, gameTime);
        }
    }
}
