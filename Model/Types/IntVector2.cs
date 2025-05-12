using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CityCourier.Model.Types;

public struct IntVector2(int x, int y) : IEquatable<IntVector2>
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public static readonly IntVector2 Up = new(0, -1);
    public static readonly IntVector2 Down = new(0, 1);
    public static readonly IntVector2 Left = new(-1, 0);
    public static readonly IntVector2 Right = new(1, 0);
    public static IntVector2 Zero => new(0, 0);

    public static readonly IntVector2[] Directions = [Up, Down, Left, Right];

    public readonly Vector2 ToVector2() => new(X, Y);

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static IntVector2 operator *(IntVector2 a, int scalar) => new(a.X * scalar, a.Y * scalar);
    public static IntVector2 operator *(IntVector2 a, double scalar) => new((int)(a.X * scalar), (int)(a.Y * scalar));
    public static IntVector2 operator /(IntVector2 a, int scalar) => new(a.X / scalar, a.Y / scalar);

    public static bool operator ==(IntVector2 a, IntVector2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(IntVector2 a, IntVector2 b) => !(a == b);

    public IEnumerable<IntVector2> GetNeighbors(bool includeDiagonals = false)
    {
        yield return Up;
        yield return Down;
        yield return Left;
        yield return Right;

        if (includeDiagonals)
        {
            yield return new IntVector2(-1, -1); // top-left
            yield return new IntVector2(1, -1); // top-right
            yield return new IntVector2(-1, 1); // bottom-left
            yield return new IntVector2(1, 1); // bottom-right
        }
    }

    public override string ToString() => $"({X}, {Y})";

    public override bool Equals(object obj) => obj is IntVector2 other && Equals(other);
    public bool Equals(IntVector2 other) => this == other;
    public override int GetHashCode() => HashCode.Combine(X, Y);
}