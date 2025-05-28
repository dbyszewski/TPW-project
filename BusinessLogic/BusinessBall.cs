using System;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  public class Ball : IBall
  {
    public Ball(Data.IBall ball)
    {
      UnderneathBall = ball;
      ball.NewPositionNotification += RaisePositionChangeEvent;
    }

    public event EventHandler<IPosition>? NewPositionNotification;
    public double Diameter => UnderneathBall.Diameter;
    public double Mass => UnderneathBall.Mass;
    public IPosition Position => new Position(UnderneathBall.Position.x, UnderneathBall.Position.y);

    internal readonly Data.IBall UnderneathBall;
    
    private void RaisePositionChangeEvent(object? sender, IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }
  }
}