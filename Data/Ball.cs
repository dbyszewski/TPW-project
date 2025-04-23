namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double diameter = DefaultDiameter)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
      Diameter = diameter;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    public double Diameter { get; private init; }

    #endregion IBall

    #region private

    private Vector Position;

    private const double DefaultDiameter = 30.0;

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
      else if (newX + Diameter >= MAX_X)
      {
        newX = MAX_X - Diameter;
        Velocity = new Vector(-Velocity.x, Velocity.y);
      }

      if (newY <= MIN_Y)
      {
        newY = MIN_Y;
        Velocity = new Vector(Velocity.x, -Velocity.y);
      }
      else if (newY + Diameter >= MAX_Y)
      {
        newY = MAX_Y - Diameter;
        Velocity = new Vector(Velocity.x, -Velocity.y);
      }

      Position = new Vector(newX, newY);

      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}
