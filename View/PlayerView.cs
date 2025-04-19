using CityCourier.Model;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace CityCourier.View;

public class PlayerView
{
    private readonly Texture2D _texture;
    private readonly Vector2 _origin;

    public PlayerView(Texture2D texture)
    {
        _texture = texture;
        _origin = new Vector2(texture.Width / 2, texture.Height / 2);
    }

    public void Draw(SpriteBatch spriteBatch, Player player)
    {
        spriteBatch.Draw(
            texture: _texture, 
            position: player.Position,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0f,
            origin: _origin,
            scale: Vector2.One, 
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
    }
}