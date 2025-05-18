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
        _infoBar.StartTime = 0;
    }
    
    public void Update(GameTime time)
    {
        if (_maze[_player.Position / MazeView.TileSize] == CellType.Parcel)
        {
            _maze[_player.Position / MazeView.TileSize] = CellType.Empty;
            _player.CollectParcel();
        }

        if ((_maze.ParcelsCount > 0 || _player.ParcelsCarrying > 0) && _player.Energy > 0)
        {
            if (_infoBar.StartTime == 0)
                _infoBar.StartTime = time.TotalGameTime.TotalSeconds;
                
            var elapsedSeconds = time.TotalGameTime.TotalSeconds - _infoBar.StartTime;
            _infoBar.Timer = $"{(int)elapsedSeconds / 60:00}:{(int)elapsedSeconds % 60:00}";
            _infoBar.CurrentState = InfoBar.State.InGame;
        }
        else
            _infoBar.CurrentState = _player.Energy <= 0 && _maze.ParcelsCount + _player.ParcelsCarrying > 0
                ? InfoBar.State.Loss
                : InfoBar.State.Win;
        
        _infoBar.CarryingParcels = _player.ParcelsCarrying;
        _infoBar.RemainedEnergy = _player.Energy;
    }
}