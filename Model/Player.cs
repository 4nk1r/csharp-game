using CityCourier.Model.Types;
using CityCourier.View;

namespace CityCourier.Model;

public class Player
{
    public IntVector2 Position { get; private set; } = new IntVector2(1, 1) * MazeView.TileSize;
    public int ParcelsCarrying { get; private set; } = 0;

    public void Move(IntVector2 direction)
    {
        Position += direction;
    }

    public void CollectParcel()
    {
        ParcelsCarrying++;
    }

    public bool DeliverParcel()
    {
        if (ParcelsCarrying <= 0) return false;
        ParcelsCarrying--;
        return true;

    }
}