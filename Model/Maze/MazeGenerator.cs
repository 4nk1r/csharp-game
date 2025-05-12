using System;
using System.Collections.Generic;
using System.Linq;
using CityCourier.Model.Types;

namespace CityCourier.Model;

public class MazeGenerator
{
    private int width, height;
    private CellType[,] grid;
    private Random random = new();

    public MazeGenerator(int width, int height)
    {
        this.width = width;
        this.height = height;
        grid = new CellType[width, height];
    }

    public CellType[,] Generate(int parcelCount)
    {
        InitializeMaze();
        GenerateMazeDFS(1, 1);

        PlaceParcels(parcelCount);
        return grid;
    }

    private void InitializeMaze()
    {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            grid[x, y] = CellType.Wall;
    }

    private void GenerateMazeDFS(int startX, int startY)
    {
        grid[startX, startY] = CellType.Empty;
        var dirs = IntVector2.Directions.OrderBy(_ => random.Next()).ToList();

        foreach (var dir in dirs)
        {
            var nx = startX + dir.X * 2;
            var ny = startY + dir.Y * 2;

            if (InBounds(nx, ny) && grid[nx, ny] == CellType.Wall)
            {
                grid[startX + dir.X, startY + dir.Y] = CellType.Empty; // carve path
                GenerateMazeDFS(nx, ny);
            }
        }
    }

    private void PlaceParcels(int count)
    {
        var emptyCells = new List<IntVector2>();

        for (var x = 1; x < width - 1; x++)
        for (var y = 1; y < height - 1; y++)
            if (grid[x, y] == CellType.Empty)
                emptyCells.Add(new IntVector2(x, y));

        if (count > emptyCells.Count)
            count = emptyCells.Count;

        for (var i = 0; i < count; i++)
        {
            var idx = random.Next(emptyCells.Count);
            var pos = emptyCells[idx];
            emptyCells.RemoveAt(idx);
            grid[pos.X, pos.Y] = CellType.Parcel;
        }
    }

    private bool InBounds(int x, int y) => x > 0 && y > 0 && x < width - 1 && y < height - 1;
}