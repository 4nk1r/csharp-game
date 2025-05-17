using CityCourier.Model;
using CityCourier.Model.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace CityCourier.View;

public class MazeView
{
    public const int TileSize = 64;

    private readonly Texture2D _houseTexture, _deliveryTarget, _floorTexture, _parcelTexture;

    public MazeView(Texture2D house, Texture2D deliveryTarget, Texture2D floor, Texture2D parcel)
    {
        _houseTexture = house;
        _deliveryTarget = deliveryTarget;
        _floorTexture = floor;
        _parcelTexture = parcel;
    }

    public void Draw(SpriteBatch spriteBatch, Maze maze)
    {
        for (var x = 0; x < maze.Width; x++)
        for (var y = 0; y < maze.Height; y++)
        {
            var position = new Vector2(x * TileSize, y * TileSize);
            var texture = maze.Grid[x, y] switch
            {
                CellType.House => _houseTexture,
                CellType.Empty => _floorTexture,
                CellType.Parcel => _parcelTexture,
                CellType.DeliveryTarget => _deliveryTarget,
                _ => _floorTexture
            };
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}