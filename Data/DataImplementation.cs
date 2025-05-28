using System.Diagnostics;
using System.Collections.Concurrent;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    public DataImplementation()
    {
      MoveTimer = new Timer(_ => Move(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
    }

    public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
        
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      Random random = new Random();
      for (int i = 0; i < numberOfBalls; i++)
      {
        double mass = random.NextDouble() * 2 + 0.5; // 0.5 to 2.5
        double diameter = DefaultDiameter * mass / 0.628;

        Vector startingPosition = new(
          random.Next((int)diameter, (int)(800 - diameter)), 
          random.Next((int)diameter, (int)(840 - diameter))
        );

        Vector initialVelocity = new(
          (random.NextDouble()) * 200, // -100 to 100
          (random.NextDouble()) * 200
        );

        Ball newBall = new(startingPosition, initialVelocity, diameter, mass);
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
    }
    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          MoveTimer.Dispose();
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    //private bool disposedValue;
    private bool Disposed = false;

    private readonly Timer MoveTimer;
    private readonly object LockObject = new();
    private readonly List<Ball> BallsList = new();
    private const double DefaultDiameter = 30.0;
    private const double TimeStep = 0.016; // 16ms

    private void Move()
    {
      if (Disposed) return;

      lock (LockObject)
      {
        foreach (var ball in BallsList)
        {
          Vector delta = new(
            ball.Velocity.x * TimeStep,
            ball.Velocity.y * TimeStep
          );
          ball.Move(delta);
        }
      }
    }

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }
  }
}
