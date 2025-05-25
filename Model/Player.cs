using CityCourier.Model.Types;
using CityCourier.View;

namespace CityCourier.Model;

public class Player
{
    public const double InitialEnergyCoefficient = 1.05;
    
    public IntVector2 Position { get; private set; } = new IntVector2(1, 1) * MazeView.TileSize;
    public int ParcelsCarrying { get; private set; }
    public int Energy { get; set; }

    public void Move(IntVector2 direction)
    {
        if (Energy > 0)
        {
            Position += direction;
            Energy--;
        }
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