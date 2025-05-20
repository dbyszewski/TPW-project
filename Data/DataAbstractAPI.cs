namespace TP.ConcurrentProgramming.Data
{
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    public static IVector CreateVector(double x, double y)
    {
      return new Vector(x, y);
    }

    #endregion Layer Factory

    #region public API

    public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);

    #endregion public API

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region Constants

    public const double TableWidth = 800.0;
    public const double TableHeight = 840.0;

    #endregion Constants

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

  public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get; init; }

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get; init; }
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;

    IVector Velocity { get; set; }
    /// <summary>
    /// The diameter of the ball.
    /// </summary>
    double Diameter { get; }
    /// <summary>
    /// The mass of the ball.
    /// </summary>
    double Mass { get; }
    /// <summary>
    /// Gets the current position of the ball.
    /// </summary>
    IVector Position { get; }
    /// <summary>
    /// Updates the velocity of the ball.
    /// </summary>
    void UpdateVelocity(IVector newVelocity);
  }
}