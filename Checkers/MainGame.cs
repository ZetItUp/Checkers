using Checkers.GameApp.Helpers;
using Checkers.GameApp.Screens;
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

        // Statisk hjälpare för att komma åt graphicsDeviceManager
        public static GraphicsDeviceManager? graphicsDeviceManager;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch? _spriteBatch;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphicsDeviceManager = _graphics;

            // Lägg till MouseHelper som en GameComponent
            Components.Add(new MouseHelper(this));
        }

        protected override void Initialize()
        {
            base.Initialize();
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.Initialize(_spriteBatch, Content);

            // Lägg till screens i ScreenManager
            ScreenManager.AddScreen(ScreenID.MainMenu, new MainMenuScreen());
            ScreenManager.AddScreen(ScreenID.Game, new GameScreen());
            ScreenManager.AddScreen(ScreenID.Replay, new ReplayScreen());

            ScreenManager.ChangeScreen(ScreenID.MainMenu);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Window.Title != WindowTitle)
            {
                Window.Title = WindowTitle;
            }

            if (ExitGame)
            {
                Exit();
            }

            ScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            ScreenManager.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
