namespace TP.ConcurrentProgramming.BusinessLogic
{
  public abstract class BusinessLogicAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region Layer API

    public static readonly Dimensions GetDimensions = new(10.0, 10.0, 10.0);
    public static readonly double TableWidth = 800.0;
    public static readonly double TableHeight = 840.0;

    public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

    public abstract void HandleCollisions();

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #endregion Layer API

    #region private

    private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());

    #endregion private
  }
  /// <summary>
  /// Immutable type representing table dimensions
  /// </summary>
  /// <param name="BallDimension"></param>
  /// <param name="TableHeight"></param>
  /// <param name="TableWidth"></param>
  /// <remarks>
  /// Must be abstract
  /// </remarks>
  public record Dimensions(double BallDimension, double TableHeight, double TableWidth);

  public interface IPosition
  {
    double x { get; init; }
    double y { get; init; }
  }

  public interface IBall 
  {
    event EventHandler<IPosition> NewPositionNotification;
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
    IPosition Position { get; }
  }
}