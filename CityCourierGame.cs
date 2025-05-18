using System.IO;
using System.Reflection;
using CityCourier.Controller;
using CityCourier.Model;
using CityCourier.View;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CityCourier;

public class CityCourierGame : Game
{
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;
    private Texture2D _restartTexture;

    private Player _player;
    private PlayerView _playerView;
    private Texture2D _playerTexture;

    private Maze _maze;
    private MazeView _mazeView;
    private Texture2D _houseTexture;
    private Texture2D _deliveryTargetTexture;
    private Texture2D _floorTexture;
    private Texture2D _parcelTexture;

    private InfoBar _infoBar;
    private InfoBarView _infoBarView;

    private InputController _inputController;
    private GameController _gameController;

    public CityCourierGame()
    {
        var graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        UpdateWindowSize(graphics);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        LoadTextures();
        InitializeGame();
    }

    private void LoadTextures()
    {
        _houseTexture = Content.Load<Texture2D>("house");
        _deliveryTargetTexture = Content.Load<Texture2D>("delivery_target");
        _floorTexture = Content.Load<Texture2D>("floor");
        _parcelTexture = Content.Load<Texture2D>("parcel");
        _playerTexture = Content.Load<Texture2D>("player");
        _restartTexture = Content.Load<Texture2D>("restart_btn");

        _fontSystem = new FontSystem();
        _fontSystem.AddFont(GetFileBytes(@"Content/Impact.ttf"));
    }

    private void InitializeGame()
    {
        _maze = new Maze();
        _mazeView = new MazeView(_houseTexture, _deliveryTargetTexture, _floorTexture, _parcelTexture);

        _player = new Player
        {
            Energy = _maze.OptimalPathLength
        };
        _playerView = new PlayerView(_playerTexture);

        _infoBar = new InfoBar();
        _infoBarView = new InfoBarView(_fontSystem, _restartTexture);

        _inputController = new InputController(_player, _maze, _infoBar);
        _gameController = new GameController(_player, _maze, _infoBar);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        if (_inputController.Update()) InitializeGame();
        else _gameController.Update(gameTime);
        
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
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var path = Path.Combine(Path.GetDirectoryName(assemblyLocation), fileName);
        return File.ReadAllBytes(path);
    }
}