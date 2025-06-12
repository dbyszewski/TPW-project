namespace TP.ConcurrentProgramming.Data
{
    public abstract class DataAbstractAPI : IDisposable
    {
        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        public static IVector CreateVector(double x, double y)
        {
            return new Vector(x, y);
        }

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler);
        public abstract void Dispose();

        public const double TableWidth = 800.0;
        public const double TableHeight = 840.0;

        public abstract ILogger GetLogger();

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());
    }

    public interface IVector
    {
        double x { get; init; }
        double y { get; init; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;
        IVector Velocity { get; set; }
        double Diameter { get; }
        double Mass { get; }
        IVector Position { get; }
        void UpdateVelocity(IVector newVelocity);
        void UpdatePosition(IVector newPosition);
    }

    public interface ILogger
    {
        void AddLog(IBall b1, IBall b2);
        void AddLog(IBall ball);
    }
}