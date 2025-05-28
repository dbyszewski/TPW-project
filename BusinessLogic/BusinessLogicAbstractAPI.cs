namespace TP.ConcurrentProgramming.BusinessLogic
{
    public abstract class BusinessLogicAbstractAPI : IDisposable
    {
        public static BusinessLogicAbstractAPI GetBusinessLogicLayer()
        {
            return modelInstance.Value;
        }

        public static readonly Dimensions GetDimensions = new(10.0, 10.0, 10.0);
        public static readonly double TableWidth = 800.0;
        public static readonly double TableHeight = 840.0;

        public abstract void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler);

        public abstract void HandleCollisions();

        public abstract void Dispose();


        private static Lazy<BusinessLogicAbstractAPI> modelInstance = new Lazy<BusinessLogicAbstractAPI>(() => new BusinessLogicImplementation());
    }

    public record Dimensions(double BallDimension, double TableHeight, double TableWidth);

    public interface IPosition
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IPosition> NewPositionNotification;
        double Diameter { get; }
        double Mass { get; }
        IPosition Position { get; }
    }
}