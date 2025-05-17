using CityCourier.Model;
using CityCourier.Model.Types;
using CityCourier.View;

namespace CityCourier.Controller;

public class GameController
{
    private Maze _maze;
    private Player _player;

    public GameController(Player player, Maze maze)
    {
        _player = player;
        _maze = maze;
    }
    
    public void Update()
    {
        if (_maze[_player.Position / MazeView.TileSize] == CellType.Parcel)
        {
            _maze[_player.Position / MazeView.TileSize] = CellType.Empty;
            _player.CollectParcel();
        }
    }
}