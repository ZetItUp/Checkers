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
        GameService? _gameService;

        Texture2D whitePiece;
        Texture2D whiteKingPiece;
        Texture2D blackPiece;
        Texture2D blackKingPiece;

        Texture2D uiTexture;

        Button btnMainMenu = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 70, 120, 50), "Main Menu");

        int boardSize = 0;
        int cellSize = 32;
        float boardScale = 1f;

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
            _lightTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, _lightColor);
            _darkTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, _darkColor);

            // Set board scale to fit window height
            boardScale = (float)MainGame.WindowHeight / (boardSize * cellSize);

            whitePiece = content.Load<Texture2D>("White");
            whiteKingPiece = content.Load<Texture2D>("WhiteKing");
            blackPiece = content.Load<Texture2D>("Black");
            blackKingPiece = content.Load<Texture2D>("BlackKing");

            uiTexture = content.Load<Texture2D>("UINormal");

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
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);
            float drawScale = (cellSize * boardScale);

            // Rita ett schackbräde
            for (int y = 0; y < boardSize; y++)
            {
                for(int x = 0; x < boardSize; x++)
                {
                    Color cellColor = ((x + y) % 2 == 0) ? _lightColor : _darkColor;
                    Texture2D cellTexture = ((x + y) % 2 == 0) ? _lightTexture : _darkTexture;
                    
                    spriteBatch.Draw(cellTexture, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, cellColor, 0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }

            spriteBatch.Draw(uiTexture, new Rectangle((int)(8 * drawScale), 0, 3*3, MainGame.WindowHeight), new Rectangle(0, 7, 3, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);


            for (int y = 0; y < boardSize; y++)
            {
                for(int x = 0; x < boardSize; x++)
                {
                    if(y == 0 && x % 2 == 1)
                    {
                        // Draw black piece
                        spriteBatch.Draw(blackPiece, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                    else if (y == 1 && x % 2 == 0)
                    {
                        // Draw white piece
                        spriteBatch.Draw(blackPiece, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                    else if (y == 2 && x % 2 == 1)
                    {
                        // Draw black piece
                        spriteBatch.Draw(blackPiece, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                    else if (y == 5 && x % 2 == 0)
                    {
                        // Draw black piece
                        spriteBatch.Draw(whitePiece, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                    else if (y == 6 && x % 2 == 1)
                    {
                        // Draw black piece
                        spriteBatch.Draw(whitePiece, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                    else if (y == 7 && x % 2 == 0)
                    {
                        // Draw white piece
                        spriteBatch.Draw(whitePiece, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
                    }
                    
                    // Draw temporary pieces to test

                }
            }

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnMainMenu.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
