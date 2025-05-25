using CityCourier.Model;
using CityCourier.Model.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace CityCourier.View;

public class MazeView(Texture2D house, Texture2D deliveryTarget, Texture2D floor, Texture2D parcel, Texture2D fence)
{
    public const int TileSize = 64;
    
    public void Draw(SpriteBatch spriteBatch, Maze maze)
    {
        for (var x = 0; x < maze.Width; x++)
        for (var y = 0; y < maze.Height; y++)
        {
            var position = new Vector2(x * TileSize, y * TileSize + InfoBarView.Height);
            var texture = maze.Grid[x, y] switch
            {
                CellType.House => house,
                CellType.Empty => floor,
                CellType.Parcel => parcel,
                CellType.DeliveryTarget => deliveryTarget,
                CellType.Fence => fence,
                _ => floor
            };
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}