using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CityCourier.Controller;

public class InputController
{
    private readonly Maze _maze;
    private readonly Player _player;
    private readonly InfoBar _infoBar;
    
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    
    public InputController(Player player, Maze maze, InfoBar infoBar)
    {
        _player = player;
        _maze = maze;
        _infoBar = infoBar;
        _previousKeyboardState = Keyboard.GetState();
        _previousMouseState = Mouse.GetState();
    }

    public bool Update()
    {
        var currentKeyboard = Keyboard.GetState();
        var currentMouse = Mouse.GetState();
        var gameRestarted = false;

        if (_infoBar.CurrentState != InfoBar.State.InGame)
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
            var currentGridPos = _player.Position / MazeView.TileSize;
            var targetGridPos = currentGridPos + direction;

            if (_maze.IsWalkable(targetGridPos)) _player.Move(direction * MazeView.TileSize);
            else if (_maze.IsDeliveryTarget(targetGridPos))
            {
                if (_player.DeliverParcel()) _maze[targetGridPos] = CellType.House;
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