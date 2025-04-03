namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double radius = DefaultRadius)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
      Radius = radius;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    #endregion IBall

    #region private

    private Vector Position;

    private const double DefaultRadius = 20.0;

    private double Radius { get; init; }

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }
    
    private const double MIN_X = 0;
    private const double MAX_X = 400;
    private const double MIN_Y = 0;
    private const double MAX_Y = 400;

    internal void Move(Vector delta)
    {
      var newX = Position.x + delta.x;
      var newY = Position.y + delta.y;

      if (newX - Radius < MIN_X)
      {
        newX = MIN_X + Radius;
        Velocity = new Vector(-Velocity.x, Velocity.y);
      }
      else if (newX + Radius > MAX_X)
      {
        newX = MAX_X - Radius;
        Velocity = new Vector(-Velocity.x, Velocity.y);
      }

      if (newY - Radius < MIN_Y)
      {
        newY = MIN_Y + Radius;
        Velocity = new Vector(Velocity.x, -Velocity.y);
      }
      else if (newY + Radius > MAX_Y)
      {
        newY = MAX_Y - Radius;
        Velocity = new Vector(Velocity.x, -Velocity.y);
      }

      Position = new Vector(newX, newY);

      
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}