namespace CityCourier.Model;

public class InfoBar
{
    public int RemainedEnergy = 999;
    public string Timer = "00:00";
    public int CarryingParcels = 0;
    public State CurrentState = State.InGame;
    public double StartTime = 0;
    
    public enum State
    {
        InGame,
        Win,
        Loss,
    }
}