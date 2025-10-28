using Checkers.GameApp.Helpers;
using Checkers.GameApp.Screens;
using Checkers.GameApp.Screens.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Checkers
{
    /// <summary>
    /// Spelklass för att initiera MonoGame 
    /// </summary>
    public class MainGame : Game
    {
        // Statiska hjälpfunktioner för att uppdatera MonoGame fönstret
        public static string WindowTitle = "Checkers Game";
        public static int WindowWidth = 1280;
        public static int WindowHeight = 720;
        public static bool ExitGame = false;

        private GraphicsDeviceManager _graphics;
        private ScreenManager? _screenManager;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Lägg till MouseHelper som en GameComponent
            Components.Add(new MouseHelper(this));
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Ändra fönstrets storlek
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            // Skapa ett nytt SpriteBatch object
            var _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Skapa ett AppContext för spelet
            var context = new AppContext(_spriteBatch, Content, _graphics.GraphicsDevice);
            
            // Skapa ScreenManager
            _screenManager = new ScreenManager(context);

            // Lägg till screens i ScreenManager
            _screenManager.AddScreen(ScreenID.MainMenu, new MainMenuScreen(_screenManager));
            _screenManager.AddScreen(ScreenID.Game, new GameScreen(_screenManager));
            _screenManager.AddScreen(ScreenID.Replay, new ReplayScreen(_screenManager));

            // Ändra screen till MainMenu
            _screenManager.ChangeScreen(ScreenID.MainMenu);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Uppdatera fönstrets titel om det har ändrats
            if (Window.Title != WindowTitle)
            {
                Window.Title = WindowTitle;
            }

            if (ExitGame)
            {
                // Låt MonoGame rensa alla resurser och avsluta programmet
                Exit();
            }

            // Uppdatera nuvarande screen
            _screenManager?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Rensa fönstret så det bara är en svart färg
            GraphicsDevice.Clear(Color.Black);

            // Rita nuvarande screen
            _screenManager?.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
