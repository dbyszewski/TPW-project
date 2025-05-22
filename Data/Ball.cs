namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double diameter = DefaultDiameter, double mass = DefaultMass)
    {
      Position = initialPosition;
      Velocity = initialVelocity;
      Diameter = diameter;
      Mass = mass;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }

    public double Diameter { get; private init; }

    public double Mass { get; private init; }

    public IVector Position { get; private set; }

    public void UpdateVelocity(IVector newVelocity)
    {
      Velocity = newVelocity;
    }

    #endregion IBall

    #region private

    private const double DefaultDiameter = 100.0;
    private const double DefaultMass = 1.0;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    internal void Move(Vector delta)
    {
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}
