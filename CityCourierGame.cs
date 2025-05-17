using System.IO;
using System.Reflection;
using CityCourier.Controller;
using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;
using FontStashSharp;
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

    private InfoBar _infoBar;
    private InfoBarView _infoBarView;

    private InputController _inputController;
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
            Content.Load<Texture2D>("house"), 
            Content.Load<Texture2D>("delivery_target"), 
            Content.Load<Texture2D>("floor"),
            Content.Load<Texture2D>("parcel"));

        _player = new Player();
        _playerView = new PlayerView(Content.Load<Texture2D>("player"));

        var fontSystem = new FontSystem();
        fontSystem.AddFont(GetFileBytes(@"Content/Impact.ttf"));
        _infoBar = new InfoBar();
        _infoBarView = new InfoBarView(fontSystem);

        _inputController = new InputController(_player, _maze);
        _gameController = new GameController(_player, _maze, _infoBar);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        _inputController.Update();
        _gameController.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(221, 197, 152));

        _spriteBatch.Begin();
        _mazeView.Draw(_spriteBatch, _maze);
        _playerView.Draw(_spriteBatch, _player);
        _infoBarView.Draw(_spriteBatch, _infoBar);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateWindowSize(GraphicsDeviceManager graphics)
    {
        graphics.PreferredBackBufferWidth = Maze.MazeWidth * MazeView.TileSize;
        graphics.PreferredBackBufferHeight = Maze.MazeHeight * MazeView.TileSize + InfoBarView.Height;
        graphics.ApplyChanges();
    }
    
    private static byte[] GetFileBytes(string fileName)
    {
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var path = Path.Combine(Path.GetDirectoryName(assemblyLocation), fileName);
        return File.ReadAllBytes(path);
    }
}