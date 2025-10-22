using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.GameService;
using Checkers.UI;

namespace Checkers.GameApp.Screens
{
    public class GameScreen : Screen
    {
        Texture2D _lightTexture;
        Texture2D _darkTexture;
        Color _lightColor = new Color(255, 255, 255);
        Color _darkColor = new Color(34, 32, 52);
        GameService _gameService;

        Button btnMainMenu = new Button(new Rectangle(276, 20, 100, 50), "Main Menu");

        int boardSize = 0;
        int cellSize = 32;

        public GameScreen()
            : base()
        {
            btnMainMenu.Clicked += BtnTest_Clicked;
        }

        private void BtnTest_Clicked(object sender, EventArgs e)
        {
            // Gå tillbaka till huvudmenyn
            ScreenManager.ChangeScreen(ScreenID.MainMenu);
        }

        public override void LoadContent(ContentManager content)
        {
            _gameService = new GameService();
            boardSize = _gameService.RuleSet.BoardSize;
            _lightTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, (int)_lightColor.PackedValue);
            _darkTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, (int)_darkColor.PackedValue);

            btnMainMenu.LoadContent(content);
        }
        public override void UnloadContent()
        {
            _gameService = null;
        }

        public override void Update(GameTime gameTime)
        {
            btnMainMenu.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap);

            // Rita ett schackbräde
            for (int y = 0; y < boardSize; y++)
            {
                for(int x = 0; x < boardSize; x++)
                {
                    Color cellColor = ((x + y) % 2 == 0) ? _lightColor : _darkColor;
                    Texture2D cellTexture = ((x + y) % 2 == 0) ? _lightTexture : _darkTexture;
                    
                    spriteBatch.Draw(cellTexture, new Rectangle(x * cellSize, y * cellSize, cellSize, cellSize), cellColor);
                }
            }

            btnMainMenu.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
