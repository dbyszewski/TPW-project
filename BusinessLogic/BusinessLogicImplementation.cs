using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using TP.ConcurrentProgramming.Data;
using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using System.Threading;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    #region ctor

    public BusinessLogicImplementation()
    {
      CollisionTimer = new Timer(async _ => await HandleCollisionsAsync(), null, 0, 32);
    }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      Data.DataAbstractAPI dataLayer = Data.DataAbstractAPI.GetDataLayer();
      dataLayer.Start(numberOfBalls, (position, ball) =>
      {
        Ball businessBall = new(ball);
        upperLayerHandler(new Position(position.x, position.y), businessBall);
        BallsList.Add(businessBall);
      });
    }

    public override void HandleCollisions()
    {
      HandleCollisionsAsync().Wait();
    }

    #endregion BusinessLogicAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          stopSource.SetResult(true);
          CollisionTimer.Dispose();
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
    }

    public override void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private readonly ConcurrentBag<Ball> BallsList = new();
    private readonly object LockObject = new();
    private bool Disposed = false;
    private readonly Timer CollisionTimer;
    private readonly TaskCompletionSource<bool> stopSource = new();

    private async Task HandleCollisionsAsync()
    {
      if (Disposed || stopSource.Task.IsCompleted)
        return;

      var balls = BallsList.ToArray();
      var collisionTasks = new List<Task>();

      for (int i = 0; i < balls.Length; i++)
      {
        for (int j = i + 1; j < balls.Length; j++)
        {
          if (stopSource.Task.IsCompleted)
            return;

          var ball1 = balls[i];
          var ball2 = balls[j];

          collisionTasks.Add(Task.Run(async () =>
          {
            if (CheckCollision(ball1, ball2))
            {
              await Task.Run(() => ResolveCollision(ball1, ball2));
            }
          }));
        }
      }

      await Task.WhenAll(collisionTasks);
    }

    private bool CheckCollision(Ball ball1, Ball ball2)
    {
      double dx = ball1.Position.x - ball2.Position.x;
      double dy = ball1.Position.y - ball2.Position.y;
      double distance = Math.Sqrt(dx * dx + dy * dy);
      return distance < (ball1.Diameter + ball2.Diameter) / 2;
    }

    private void ResolveCollision(Ball ball1, Ball ball2)
    {
      // Calculate collision normal
      double dx = ball2.Position.x - ball1.Position.x;
      double dy = ball2.Position.y - ball1.Position.y;
      double distance = Math.Sqrt(dx * dx + dy * dy);
      double nx = dx / distance;
      double ny = dy / distance;

      // Calculate relative velocity
      double vx = ball2.UnderneathBall.Velocity.x - ball1.UnderneathBall.Velocity.x;
      double vy = ball2.UnderneathBall.Velocity.y - ball1.UnderneathBall.Velocity.y;
      double relativeVelocity = vx * nx + vy * ny;

      // Don't resolve if balls are moving apart
      if (relativeVelocity > 0)
        return;

      // Calculate impulse
      double restitution = 1.0; // Perfectly elastic collision
      double impulse = -(1 + restitution) * relativeVelocity;
      impulse /= 1 / ball1.Mass + 1 / ball2.Mass;

      // Apply impulse
      ball1.UnderneathBall.UpdateVelocity(Data.DataAbstractAPI.CreateVector(
        ball1.UnderneathBall.Velocity.x - (impulse * nx / ball1.Mass),
        ball1.UnderneathBall.Velocity.y - (impulse * ny / ball1.Mass)
      ));

      ball2.UnderneathBall.UpdateVelocity(Data.DataAbstractAPI.CreateVector(
        ball2.UnderneathBall.Velocity.x + (impulse * nx / ball2.Mass),
        ball2.UnderneathBall.Velocity.y + (impulse * ny / ball2.Mass)
      ));
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}