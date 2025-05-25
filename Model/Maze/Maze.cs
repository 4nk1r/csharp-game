using CityCourier.Model.Types;

namespace CityCourier.Model;

public class Maze
{
    public CellType[,] Grid { get; }
    public readonly int OptimalPathLength;

    public int ParcelsCount
    {
        get
        {
            var count = 0;
            for (var x = 0; x < CityCourierGame.MazeWidth; x++)
            for (var y = 0; y < CityCourierGame.MazeHeight; y++)
                if (Grid[x, y] == CellType.Parcel)
                    count++;
            return count;
        }
    }

    public int Width => Grid.GetLength(0);
    public int Height => Grid.GetLength(1);

    public Maze()
    {
        var generator = new MazeGenerator(CityCourierGame.MazeWidth, CityCourierGame.MazeHeight);
        var solver = new MazeSolver();
        
        Grid = generator.Generate(CityCourierGame.ParcelCount);
        OptimalPathLength = solver.GetShortestPathLength(Grid);
    }

    public void OpenFences()
    {
        for (var x = 0; x < Width; x++)
        for (var y = 0; y < Height; y++)
            if (Grid[x, y] == CellType.Fence)
                Grid[x, y] = CellType.Empty;
    }

    public bool IsWalkable(IntVector2 target) => IsWithinBounds(target) 
                                                 && this[target] != CellType.House 
                                                 && this[target] != CellType.DeliveryTarget
                                                 && this[target] != CellType.Fence;

    public bool IsDeliveryTarget(IntVector2 target) => IsWithinBounds(target) && this[target] == CellType.DeliveryTarget;

    public CellType this[IntVector2 pos]
    {
        get => Grid[pos.X, pos.Y];
        set => Grid[pos.X, pos.Y] = value;
    }

    private bool IsWithinBounds(IntVector2 pos) => pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;
}