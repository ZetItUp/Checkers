using Checkers.CheckersGame.DataTypes;
using Checkers.CheckersGame.GameService;
using Checkers.CheckersGame.History;
using Checkers.CheckersGame.Models;
using Checkers.GameApp.Helpers;
using Checkers.GameApp.UI;
using Checkers.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.GameApp.Screens
{
    public class ReplayScreen : Screen
    {
        Texture2D? _lightTexture;
        Texture2D? _darkTexture;
        Color _lightColor = new Color(255, 255, 255);
        Color _darkColor = new Color(34, 32, 52);

        ReplayService? _replayService;

        Texture2D? whitePiece;
        Texture2D? whiteKingPiece;
        Texture2D? blackPiece;
        Texture2D? blackKingPiece;

        Texture2D? selectTexture;
        Texture2D? validMoveTexture;
        Texture2D? uiTexture;

        Button? btnMainMenu = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 70, 120, 50), "Main Menu");
        Button? btnAutoPlay = new Button(new Rectangle(MainGame.WindowWidth - 130, MainGame.WindowHeight - 130, 120, 50), "Auto Play: ON");
        Button? btnReset = new Button(new Rectangle(MainGame.WindowWidth - 260, MainGame.WindowHeight - 70, 120, 50), "Reset");
        Button? btnNextMove = new Button(new Rectangle(MainGame.WindowWidth - 390, MainGame.WindowHeight - 70, 120, 50), "Next >>");
        Button? btnPreviousMove = new Button(new Rectangle(MainGame.WindowWidth - 520, MainGame.WindowHeight - 70, 120, 50), "<< Previous");
        ComboBox? cboGames = new ComboBox(new Rectangle(MainGame.WindowWidth - 410, 10, 400, 40));

        int boardSize = 8;
        int cellSize = 32;
        float boardScale = 1f;
        float drawScale = 1f;

        float autoPlayTimer = 500f;
        float lastAutoPlayTime = 0f;
        bool isAutoPlaying = false;

        public ReplayScreen()
            : base()
        {
            btnMainMenu.Clicked += BtnMainMenu_Clicked;
            btnNextMove.Clicked += BtnNextMove_Clicked;
            btnPreviousMove.Clicked += BtnPreviousMove_Clicked;
            btnReset.Clicked += BtnReset_Clicked;
            btnAutoPlay.Clicked += btnAutoPlay_Clicked;
            cboGames.SelectedItemChanged += CboGames_SelectedItemChanged;
        }

        private void btnAutoPlay_Clicked(object? sender, EventArgs e)
        {
            isAutoPlaying = !isAutoPlaying;

            if (btnAutoPlay != null)
            {
                if (isAutoPlaying)
                {
                    btnAutoPlay.Text = "Auto Play: ON";
                }
                else
                {
                    btnAutoPlay.Text = "Auto Play: OFF";
                }
            }
        }

        private void BtnReset_Clicked(object? sender, EventArgs e)
        {
            _replayService?.ResetToStart();
        }

        private void BtnPreviousMove_Clicked(object? sender, EventArgs e)
        {
            _replayService?.StepBackward();
        }

        private void BtnNextMove_Clicked(object? sender, EventArgs e)
        {
            _replayService?.StepForward();
        }

        private void CboGames_SelectedItemChanged(object? sender, EventArgs e)
        {
            string? fileName = cboGames?.SelectedItemText;

            if(string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var savedGame = GamePersistence.LoadGameForReplay(fileName);

            if (savedGame == null)
            {
                return;
            }

            _replayService = new ReplayService(savedGame);
            _replayService.ResetToStart();
        }

        private void BtnMainMenu_Clicked(object? sender, EventArgs e)
        {
            // Gå tillbaka till huvudmenyn
            ScreenManager.ChangeScreen(ScreenID.MainMenu);
        }

        public override void LoadContent(ContentManager content)
        {
            if(MainGame.graphicsDeviceMangager == null)
            {
                throw new Exception("GraphicsDeviceManager is not initialized.");
            }

            _lightTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, _lightColor);
            _darkTexture = GraphicsHelper.CreateTexture(MainGame.graphicsDeviceMangager.GraphicsDevice, cellSize, cellSize, _darkColor);

            // Set board scale to fit window height
            boardScale = (float)MainGame.WindowHeight / (boardSize * cellSize);
            drawScale = (cellSize * boardScale);

            whitePiece = content.Load<Texture2D>("White");
            whiteKingPiece = content.Load<Texture2D>("WhiteKing");
            blackPiece = content.Load<Texture2D>("Black");
            blackKingPiece = content.Load<Texture2D>("BlackKing");

            validMoveTexture = content.Load<Texture2D>("Move");
            uiTexture = content.Load<Texture2D>("UINormal");
            selectTexture = content.Load<Texture2D>("Select");

            btnMainMenu?.LoadContent(content);
            btnReset?.LoadContent(content);
            btnNextMove?.LoadContent(content);
            btnPreviousMove?.LoadContent(content);
            btnAutoPlay?.LoadContent(content);

            if (cboGames != null)
            {
                cboGames.LoadContent(content);
                cboGames.SelectedItemText = "<Select a Previous Game>";
            }

            // Ladda in sparade spel för replay
            var savedGames = GamePersistence.GetSavedGamesWithMetadata();
            foreach (var game in savedGames)
            {
                cboGames?.AddItem(game.FileName);
            }
        }
        public override void UnloadContent()
        {
            _replayService = null;
            btnMainMenu?.UnloadContent();
            btnReset?.UnloadContent();
            btnNextMove?.UnloadContent();
            btnPreviousMove?.UnloadContent();
            btnAutoPlay?.UnloadContent();
            cboGames?.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            btnMainMenu?.Update(gameTime);
            btnReset?.Update(gameTime);
            btnNextMove?.Update(gameTime);
            btnPreviousMove?.Update(gameTime);
            btnAutoPlay?.Update(gameTime);
            cboGames?.Update(gameTime);

            if(_replayService == null)
            {
                return;
            }
            if (btnNextMove != null)
            {
                btnNextMove.Enabled = _replayService != null && !_replayService.IsAtEnd && !isAutoPlaying;
            }
            if (btnPreviousMove != null)
            {
                btnPreviousMove.Enabled = _replayService != null && !_replayService.IsAtStart && !isAutoPlaying;
            }
            if (btnAutoPlay != null)
            {
                btnAutoPlay.Enabled = _replayService != null;
            }

            if (isAutoPlaying)
            {
                lastAutoPlayTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (lastAutoPlayTime >= autoPlayTimer)
                {
                    lastAutoPlayTime = 0f;
                    if(_replayService != null && !_replayService.IsAtEnd)
                    {
                        _replayService.StepForward();
                    }
                    else
                    {
                        isAutoPlaying = false;

                        if (btnAutoPlay != null)
                        {
                            btnAutoPlay.Text = "Auto Play: OFF";
                        }
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            // Rita ett schackbräde
            for (int y = 0; y < boardSize; y++)
            {
                for (int x = 0; x < boardSize; x++)
                {
                    Color cellColor = ((x + y) % 2 == 0) ? _lightColor : _darkColor;
                    Texture2D? cellTexture = ((x + y) % 2 == 0) ? _lightTexture : _darkTexture;

                    spriteBatch.Draw(cellTexture, new Rectangle((int)(x * drawScale), (int)(y * drawScale), (int)drawScale, (int)drawScale), null, cellColor, 0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                }
            }

            if (_replayService != null)
            {
                var board = _replayService.GetBoard();
                var pieces = board?.GetAllPieces();

                if (pieces != null)
                {
                    for (int i = 0; i < pieces.Count; i++)
                    {
                        var piece = pieces[i];
                        Texture2D? pieceTexture = null;
                        if (piece.Color == PieceColor.Red)
                        {
                            pieceTexture = piece is KingPiece ? whiteKingPiece : whitePiece;
                        }
                        else
                        {
                            pieceTexture = piece is KingPiece ? blackKingPiece : blackPiece;
                        }
                        spriteBatch.Draw(pieceTexture, new Rectangle((int)(piece.Position.Column * drawScale), (int)(piece.Position.Row * drawScale), (int)drawScale, (int)drawScale), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                    }
                }
            }


            spriteBatch.Draw(uiTexture, new Rectangle((int)(boardSize * drawScale), 0, 3 * 3, MainGame.WindowHeight), new Rectangle(0, 7, 3, 1), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred);
            btnMainMenu?.Draw(spriteBatch);
            btnReset?.Draw(spriteBatch);
            btnNextMove?.Draw(spriteBatch);
            btnPreviousMove?.Draw(spriteBatch);
            btnAutoPlay?.Draw(spriteBatch);

            cboGames?.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
