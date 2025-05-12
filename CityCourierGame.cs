using CityCourier.Controller;
using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CityCourier;

public class CityCourierGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Player _player;
    private PlayerView _playerView;

    private Maze _maze;
    private MazeView _mazeView;

    private GameController _gameController;

    public CityCourierGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        UpdateWindowSize(_graphics);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _maze = new Maze();
        _mazeView = new MazeView(
            Content.Load<Texture2D>("wall"), 
            Content.Load<Texture2D>("floor"),
            Content.Load<Texture2D>("parcel"));

        _player = new Player();
        _playerView = new PlayerView(Content.Load<Texture2D>("player"));

        _gameController = new GameController(_player, _maze);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        _gameController.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        _mazeView.Draw(_spriteBatch, _maze);
        _playerView.Draw(_spriteBatch, _player);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateWindowSize(GraphicsDeviceManager graphics)
    {
        graphics.PreferredBackBufferWidth = Maze.MazeWidth * MazeView.TileSize;
        graphics.PreferredBackBufferHeight = Maze.MazeHeight * MazeView.TileSize;
        graphics.ApplyChanges();
    }
}