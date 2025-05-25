using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CityCourier.Controller;

public class InputController(Player player, Maze maze, InfoBar infoBar)
{
    private KeyboardState _previousKeyboardState = Keyboard.GetState();
    private MouseState _previousMouseState = Mouse.GetState();

    public bool Update()
    {
        var currentKeyboard = Keyboard.GetState();
        var currentMouse = Mouse.GetState();
        var gameRestarted = false;

        if (infoBar.CurrentState != InfoBar.State.InGame)
            if (IsMouseClickedOnce(currentMouse, _previousMouseState))
            {
                var restartButtonBounds = new Rectangle(
                    InfoBarView.RestartButtonPosition.X,
                    InfoBarView.RestartButtonPosition.Y,
                    InfoBarView.RestartButtonSize.width,
                    InfoBarView.RestartButtonSize.height
                );
                gameRestarted = restartButtonBounds.Contains(currentMouse.Position);
            }
        
        var direction = GetPlayerDirection(_previousKeyboardState, currentKeyboard);
        if (direction != IntVector2.Zero)
        {
            var currentGridPos = player.Position / MazeView.TileSize;
            var targetGridPos = currentGridPos + direction;

            if (maze.IsWalkable(targetGridPos)) player.Move(direction * MazeView.TileSize);
            else if (maze.IsDeliveryTarget(targetGridPos))
            {
                if (player.DeliverParcel())
                {
                    maze[targetGridPos] = CellType.House;
                    maze.OpenFences();
                }
            }
        }
        _previousKeyboardState = currentKeyboard;
        _previousMouseState = currentMouse;
        
        return gameRestarted;
    }

    private static IntVector2 GetPlayerDirection(KeyboardState previous, KeyboardState current)
    {
        if (IsKeyPressedOnce(previous, current, Keys.W) || IsKeyPressedOnce(previous, current, Keys.Up)) 
            return IntVector2.Up;
        if (IsKeyPressedOnce(previous, current, Keys.S) || IsKeyPressedOnce(previous, current, Keys.Down)) 
            return IntVector2.Down;
        if (IsKeyPressedOnce(previous, current, Keys.A) || IsKeyPressedOnce(previous, current, Keys.Left)) 
            return IntVector2.Left;
        if (IsKeyPressedOnce(previous, current, Keys.D) || IsKeyPressedOnce(previous, current, Keys.Right)) 
            return IntVector2.Right;
        
        return IntVector2.Zero;
    }
    
    private static bool IsKeyPressedOnce(KeyboardState previous, KeyboardState current, Keys key) =>
        current.IsKeyDown(key) && previous.IsKeyUp(key);

    private static bool IsMouseClickedOnce(MouseState current, MouseState previous) =>
        current.LeftButton == ButtonState.Released && previous.LeftButton == ButtonState.Pressed;
}