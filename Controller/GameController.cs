using CityCourier.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CityCourier.Controller;

public class GameController
{
    private const float PlayerSpeed = 200f;

    private Player _player;

    public GameController(Player player)
    {
        _player = player;
    }

    public void Update(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();

        _player.Move(GetPlayerDirection(keyboard) * (float)gameTime.ElapsedGameTime.TotalSeconds * PlayerSpeed);
    }

    private static Vector2 GetPlayerDirection(KeyboardState keyboard)
    {
        var direction = Vector2.Zero;

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up)) direction.Y -= 1;
        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down)) direction.Y += 1;
        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left)) direction.X -= 1;
        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right)) direction.X += 1;

        return direction;
    }
}