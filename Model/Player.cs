using CityCourier.Model.Types;
using CityCourier.View;

namespace CityCourier.Model;

public class Player
{
    public IntVector2 Position { get; private set; } = new IntVector2(1, 1) * MazeView.TileSize;

    public void Move(IntVector2 direction)
    {
        Position += direction;
    }
}