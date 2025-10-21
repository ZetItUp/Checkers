using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checkers.CheckersGame.GameService;

namespace Checkers.GameApp.Screens
{
    public class GameScreen : Screen
    {
        Texture2D _lightTexture;
        Texture2D _darkTexture;
        GameService _gameService;

        int boardSize = 0;

        public GameScreen()
            : base()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            _gameService = new GameService("Player 1", "Player 2");
            boardSize = _gameService.RuleSet.BoardSize;

        }
        public override void UnloadContent()
        {
            _gameService = null;
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

        }
    }
}
