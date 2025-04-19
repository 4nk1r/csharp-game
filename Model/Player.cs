using Microsoft.Xna.Framework;

namespace CityCourier.Model;

public class Player
{
    public Vector2 Position { get; private set; }
    
    public void Move(Vector2 direction)
    {
        Position += direction;
    }
}