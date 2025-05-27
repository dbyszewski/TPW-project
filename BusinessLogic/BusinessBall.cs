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

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;
    public double Diameter => UnderneathBall.Diameter;
    public double Mass => UnderneathBall.Mass;
    public IPosition Position => new Position(UnderneathBall.Position.x, UnderneathBall.Position.y);

    #endregion IBall

    #region private
    internal readonly Data.IBall UnderneathBall;

    public void Move(double deltaX, double deltaY)
    {
      // Tworzymy wektor przesunięcia i wywołujemy metodę Move z interfejsu Data.IBall
      //UnderneathBall.Move(Data.DataAbstractAPI.CreateVector(deltaX, deltaY));
    }
    
    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    #endregion private
  }
}