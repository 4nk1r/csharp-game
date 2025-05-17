using CityCourier.Model;
using CityCourier.Model.Types;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace CityCourier.View;

public class PlayerView
{
    private readonly Texture2D _texture;
    private readonly IntVector2 _origin;

    private Vector2 _infoBarOffset = new(0, InfoBarView.Height);

    public PlayerView(Texture2D texture)
    {
        _texture = texture;
        _origin = IntVector2.Zero;
    }

    public void Draw(SpriteBatch spriteBatch, Player player)
    {
        spriteBatch.Draw(
            texture: _texture, 
            position: player.Position.ToVector2() + _infoBarOffset,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0f,
            origin: _origin.ToVector2(),
            scale: Vector2.One, 
            effects: SpriteEffects.None,
            layerDepth: 0f
        );
    }
}