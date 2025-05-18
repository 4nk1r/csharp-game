using CityCourier.Model;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CityCourier.View;

public class InfoBarView(FontSystem fontSystem, Texture2D restart)
{
    public const int Height = 50;
    public static Point RestartButtonPosition = new(0, 4);
    public static (int width, int height) RestartButtonSize = (82, 42);

    public void Draw(SpriteBatch spriteBatch, InfoBar infoBar)
    {
        SpriteFontBase font = fontSystem.GetFont(Height - 16);
        spriteBatch.DrawString(
            font,
            InfoText(infoBar),
            new Vector2(16, 8),
            new Color(48, 29, 26)
        );
        
        if (infoBar.CurrentState != InfoBar.State.InGame)
        {
            RestartButtonPosition = new Point(spriteBatch.GraphicsDevice.Viewport.Width - 90, 4);
            spriteBatch.Draw(
                restart,
                new Vector2(RestartButtonPosition.X, RestartButtonPosition.Y),
                new Rectangle(0, 0, RestartButtonSize.width, RestartButtonSize.height),
                Color.White
            );
        }
    }

    private static string InfoText(InfoBar infoBar)
    {
        return $"Время: {infoBar.Timer} {new string(' ', 10)} " +
               $"Энергия: {infoBar.RemainedEnergy} {new string(' ', 10)} " +
               infoBar.CurrentState switch
               {
                   InfoBar.State.Loss => "ПОРАЖЕНИЕ",
                   InfoBar.State.Win => "ПОБЕДА",
                   _ => $"Посылок в инвентаре: {infoBar.CarryingParcels}"
               };
    }
}