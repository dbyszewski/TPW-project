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

    private const double DefaultRadius = 15.0;

    private double Radius { get; init; }

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }
    
    private const double MIN_X = 0;
    private const double MAX_X = 400;
    private const double MIN_Y = 0;
    private const double MAX_Y = 420;

    internal void Move(Vector delta)
    {
      var newX = Position.x + delta.x;
      var newY = Position.y + delta.y;

      if (newX <= MIN_X)
      {
        newX = MIN_X;
        Velocity = new Vector(-Velocity.x, Velocity.y);
      }
      else if (newX + 2 * Radius >= MAX_X)
      {
        newX = MAX_X - 2 * Radius;
        Velocity = new Vector(-Velocity.x, Velocity.y);
      }

      if (newY <= MIN_Y)
      {
        newY = MIN_Y;
        Velocity = new Vector(Velocity.x, -Velocity.y);
      }
      else if (newY + 2 * Radius >= MAX_Y)
      {
        newY = MAX_Y - 2 * Radius;
        Velocity = new Vector(Velocity.x, -Velocity.y);
      }

      Position = new Vector(newX, newY);

      
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}