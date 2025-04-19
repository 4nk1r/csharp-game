using CityCourier.Controller;
using CityCourier.Model;
using CityCourier.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CityCourier;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Player player;
    PlayerView playerView;
    GameController gameController;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        player = new Player();
        playerView = new PlayerView(Content.Load<Texture2D>("ball"));

        gameController = new GameController(player);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        
        gameController.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();
        playerView.Draw(_spriteBatch, player);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}