using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;
using Microsoft.Xna.Framework;

namespace CityCourier.Controller;

public class GameController
{
    private Maze _maze;
    private Player _player;
    private InfoBar _infoBar;

    public GameController(Player player, Maze maze, InfoBar infoBar)
    {
        _player = player;
        _maze = maze;
        _infoBar = infoBar;
    }
    
    public void Update(GameTime gameTime)
    {
        if (_maze[_player.Position / MazeView.TileSize] == CellType.Parcel)
        {
            _maze[_player.Position / MazeView.TileSize] = CellType.Empty;
            _player.CollectParcel();
        }

        if (_maze.ParcelsCount > 0 || _player.ParcelsCarrying > 0)
            _infoBar.Timer =
                $"{(int)gameTime.TotalGameTime.TotalMinutes:00}:{(int)gameTime.TotalGameTime.TotalSeconds % 60:00}";
        _infoBar.CarryingParcels = _player.ParcelsCarrying;
    }
}