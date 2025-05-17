using CityCourier.Model.Types;

namespace CityCourier.Model;

public class Maze
{
    public const int MazeWidth = 25;
    public const int MazeHeight = 13;
    private const int ParcelCount = 6;

    public CellType[,] Grid { get; }

    public int Width => Grid.GetLength(0);
    public int Height => Grid.GetLength(1);

    public Maze()
    {
        var generator = new MazeGenerator(MazeWidth, MazeHeight);
        Grid = generator.Generate(ParcelCount);
    }

    public bool IsWalkable(IntVector2 target) => 
        IsWithinBounds(target) && this[target] != CellType.House && this[target] != CellType.DeliveryTarget;

    public bool IsDeliveryTarget(IntVector2 target) => IsWithinBounds(target) && this[target] == CellType.DeliveryTarget;
    
    public bool IsWithinBounds(IntVector2 pos) => pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;

    public CellType this[IntVector2 pos]
    {
        get => Grid[pos.X, pos.Y];
        set => Grid[pos.X, pos.Y] = value;
    }
}