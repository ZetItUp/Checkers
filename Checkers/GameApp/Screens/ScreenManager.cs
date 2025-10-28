using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Checkers.GameApp.Screens.Interfaces;

namespace Checkers.GameApp.Screens
{
    // Klass för att hantera olika screens i spelet
    public sealed class ScreenManager : IScreenChanger
    {
        private readonly IAppContext _appContext;

        // Nuvarande aktiva screen
        private IScreen? _currentScreen;
        // ContentManager för att ladda innehåll
        private ContentManager? _content;
        // SpriteBatch för att rita grafik
        private SpriteBatch? _spriteBatch;
        // Dictionary för att lagra olika screens, mappade till deras ScreenID
        private readonly Dictionary<ScreenID, IScreen> _screens = new Dictionary<ScreenID, IScreen>();

        public ScreenManager(IAppContext context) 
        {
            _appContext = context;
        }

        /// <summary>
        /// Hämta nuvarande aktiva screen
        /// </summary>
        public IScreen CurrentScreen
        {
            get 
            { 
                return _currentScreen!; 
            }
        }

        /// <summary>
        /// Lägg till en screen i ScreenManager
        /// </summary>
        /// <param name="screenID">ScreenID for screen, must be unique</param>
        /// <param name="screen">Screen object</param>
        public void AddScreen(ScreenID screenID, IScreen screen)
        {
            _screens.Add(screenID, screen);
        }

        /// <summary>
        /// Byt screen till den angivna ScreenID
        /// </summary>
        /// <param name="screenID">ScreenID för den skärm som önskas bytas till</param>
        public void ChangeScreen(ScreenID screenID)
        {
            // Töm nuvarande screen om den finns
            _currentScreen?.UnloadContent();

            // Kolla så att screenID finns i _screens
            if(_screens.ContainsKey(screenID))
            {
                // Sätt aktiv screen till den nya och ladda dess innehåll
                _currentScreen = _screens[screenID];
                _currentScreen.LoadContent(_appContext);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Uppdatera nuvarande screen om den finns
            _currentScreen?.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            // Rita nuvarande screen om den finns
            _currentScreen?.Draw(gameTime);
        }
    }
}
