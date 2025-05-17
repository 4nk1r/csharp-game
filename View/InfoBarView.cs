using CityCourier.Model;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CityCourier.View;

public class InfoBarView
{
    public const int Height = 50;
    private FontSystem _fontSystem;

    public InfoBarView(FontSystem fontSystem)
    {
        _fontSystem = fontSystem;
    }

    public void Draw(SpriteBatch spriteBatch, InfoBar infoBar)
    {
        SpriteFontBase font = _fontSystem.GetFont(Height - 16);
        spriteBatch.DrawString(
            font,
            $"Время: {infoBar.Timer} {new string(' ', 10)} Посылок в инвентаре: {infoBar.CarryingParcels}",
            new Vector2(16, 8),
            new Color(48, 29, 26)
        );
    }
}