using System;
using System.Collections.Generic;
using System.Linq;
using CityCourier.Model.Types;

namespace CityCourier.Model;

public class MazeGenerator(int width, int height)
{
    private readonly CellType[,] _grid = new CellType[width, height];
    private readonly Random _random = new();

    public CellType[,] Generate(int parcelCount)
    {
        InitializeMaze();
        GenerateMazeDfs(1, 1);

        PlaceParcels(parcelCount);
        PlaceDeliveryTargets(parcelCount);
        PlaceFences(CityCourierGame.FenceCount);
        return _grid;
    }

    private void InitializeMaze()
    {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            _grid[x, y] = CellType.House;
    }

    private void GenerateMazeDfs(int startX, int startY)
    {
        _grid[startX, startY] = CellType.Empty;
        var dirs = IntVector2.Directions.OrderBy(_ => _random.Next()).ToList();

        foreach (var dir in dirs)
        {
            var nx = startX + dir.X * 2;
            var ny = startY + dir.Y * 2;

            if (InBounds(nx, ny) && _grid[nx, ny] == CellType.House)
            {
                _grid[startX + dir.X, startY + dir.Y] = CellType.Empty; // carve path
                GenerateMazeDfs(nx, ny);
            }
        }
    }

    // randomly replaces n houses with delivery targets
    private void PlaceDeliveryTargets(int n)
    {
        var wallCells = new List<IntVector2>();
        for (var x = 1; x < width - 1; x++)
        for (var y = 1; y < height - 1; y++)
            if (_grid[x, y] == CellType.House)
                wallCells.Add(new IntVector2(x, y));

        n = Math.Min(n, wallCells.Count);

        for (var i = 0; i < n; i++)
        {
            var idx = _random.Next(wallCells.Count);
            var pos = wallCells[idx];
            wallCells.RemoveAt(idx);

            _grid[pos.X, pos.Y] = CellType.DeliveryTarget;
        }
    }

    // tries to place parcels in dead-ends, randomly otherwise
    private void PlaceParcels(int count)
    {
        var deadEnds = new Stack<IntVector2>();
        for (var x = 1; x < width - 1; x++)
        for (var y = 1; y < height - 1; y++)
        {
            if ((x == 1 && y == 1) || _grid[x, y] != CellType.Empty)
                continue;

            var neighbors = 0;
            foreach (var dir in IntVector2.Directions)
            {
                int nx = x + dir.X, ny = y + dir.Y;
                if (InBounds(nx, ny) && _grid[nx, ny] == CellType.Empty)
                    neighbors++;
            }

            if (neighbors == 1) deadEnds.Push(new IntVector2(x, y));
        }

        var placed = 0;
        while (placed < count && deadEnds.Count > 0)
        {
            var pos = deadEnds.Pop();
            _grid[pos.X, pos.Y] = CellType.Parcel;
            placed++;
        }

        if (placed < count)
        {
            var emptyCells = new List<IntVector2>();
            for (var x = 1; x < width - 1; x++)
            for (var y = 1; y < height - 1; y++)
            {
                if ((x == 1 && y == 1) || _grid[x, y] != CellType.Empty)
                    continue;
                emptyCells.Add(new IntVector2(x, y));
            }

            while (placed < count && emptyCells.Count > 0)
            {
                var idx = _random.Next(emptyCells.Count);
                var pos = emptyCells[idx];
                emptyCells.RemoveAt(idx);

                _grid[pos.X, pos.Y] = CellType.Parcel;
                placed++;
            }
        }
    }

    // places fences in positions where houses have empty cells on both sides horizontally or vertically
    private void PlaceFences(int n)
    {
        var potentialFences = new List<IntVector2>();

        for (var x = 1; x < width - 1; x++)
        for (var y = 1; y < height - 1; y++)
        {
            if (_grid[x, y] != CellType.House) continue;

            if (InBounds(x - 1, y) && InBounds(x + 1, y) && InBounds(x, y - 1) && InBounds(x, y + 1)
                && _grid[x - 1, y] == CellType.Empty && _grid[x + 1, y] == CellType.Empty
                && _grid[x, y - 1] != CellType.Empty && _grid[x, y + 1] != CellType.Empty)
                potentialFences.Add(new IntVector2(x, y));

            if (InBounds(x - 1, y) && InBounds(x + 1, y) && InBounds(x, y - 1) && InBounds(x, y + 1)
                && _grid[x - 1, y] != CellType.Empty && _grid[x + 1, y] != CellType.Empty
                && _grid[x, y - 1] == CellType.Empty && _grid[x, y + 1] == CellType.Empty)
                potentialFences.Add(new IntVector2(x, y));
        }

        n = Math.Min(n, potentialFences.Count);
        for (var i = 0; i < n; i++)
        {
            var idx = _random.Next(potentialFences.Count);
            var pos = potentialFences[idx];
            potentialFences.RemoveAt(idx);

            _grid[pos.X, pos.Y] = CellType.Fence;
        }
    }

    private bool InBounds(int x, int y) => x > 0 && y > 0 && x < width - 1 && y < height - 1;
}