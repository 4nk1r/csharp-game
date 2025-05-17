using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;
using Microsoft.Xna.Framework.Input;

namespace CityCourier.Controller;

public class InputController
{
    private Maze _maze;
    private Player _player;
    private KeyboardState _previousKeyboardState;

    public InputController(Player player, Maze maze)
    {
        _player = player;
        _maze = maze;
        _previousKeyboardState = Keyboard.GetState(); 
    }

    public void Update()
    {
        var currentKeyboard = Keyboard.GetState();

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
}