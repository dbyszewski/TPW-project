using System.Collections.Concurrent;
using System.Diagnostics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation()
        {
            this.dataLayer = Data.DataAbstractAPI.GetDataLayer();
            CollisionTimer = new Timer(async _ => await HandleCollisionsAsync(), null, 0, 16);
        }

        public BusinessLogicImplementation(Data.DataAbstractAPI dataLayer)
        {
            this.dataLayer = dataLayer;
            CollisionTimer = new Timer(async _ => await HandleCollisionsAsync(), null, 0, 16);
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Data.DataAbstractAPI dataLayer = this.dataLayer ?? Data.DataAbstractAPI.GetDataLayer();
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
                    CollisionTimer.Dispose();
                    BallsList.Clear();
                    checkedCollisions.Clear();
                    dataLayer?.Dispose();
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

        private readonly Data.DataAbstractAPI? dataLayer;
        private readonly ConcurrentBag<Ball> BallsList = new();
        private readonly object LockObject = new();
        private bool Disposed = false;
        private readonly Timer CollisionTimer;
        private readonly ConcurrentDictionary<(Ball, Ball), bool> checkedCollisions = new();

        private const double MIN_X = 0;
        private const double MAX_X = 796;
        private const double MIN_Y = 0;
        private const double MAX_Y = 836;

        private bool isProcessingCollisions = false;

        private async Task HandleCollisionsAsync()
        {
            if (Disposed)
                return;

            if (isProcessingCollisions)
                return;

            try
            {
                isProcessingCollisions = true;
                checkedCollisions.Clear();

                var balls = BallsList.ToArray();
                var tasks = new List<Task>();
                var detectedCollisions = new ConcurrentBag<(Ball, Ball)>();

                foreach (var ball in balls)
                {
                    tasks.Add(Task.Run(() => HandleWallCollisions(ball)));
                }

                for (int i = 0; i < balls.Length; i++)
                {
                    var iCopy = i;
                    tasks.Add(Task.Run(() =>
                    {
                        for (int j = iCopy + 1; j < balls.Length; j++)
                        {
                            var ball1 = balls[iCopy];
                            var ball2 = balls[j];

                            if (checkedCollisions.ContainsKey((ball1, ball2)) || checkedCollisions.ContainsKey((ball2, ball1)))
                                continue;

                            checkedCollisions.TryAdd((ball1, ball2), true);

                            if (CheckCollision(ball1, ball2))
                            {
                                detectedCollisions.Add((ball1, ball2));
                            }
                        }
                    }));
                }

                await Task.WhenAll(tasks);

                // Rozwiązujemy wszystkie wykryte kolizje asynchronicznie
                var resolveTasks = new List<Task>();
                foreach (var (ball1, ball2) in detectedCollisions)
                {
                    resolveTasks.Add(Task.Run(() => ResolveCollision(ball1, ball2)));
                }
                await Task.WhenAll(resolveTasks);
            }
            finally
            {
                isProcessingCollisions = false;
            }
        }

        private void HandleWallCollisions(Ball ball)
        {
            lock (LockObject)
            {
                var position = ball.UnderneathBall.Position;
                var velocity = ball.UnderneathBall.Velocity;
                var diameter = ball.UnderneathBall.Diameter;
                var newX = position.x;
                var newY = position.y;
                var newVelocityX = velocity.x;
                var newVelocityY = velocity.y;

                if (newX < MIN_X && velocity.x < 0)
                {
                    newX = MIN_X;
                    newVelocityX = -velocity.x;
                }
                else if (newX + diameter > MAX_X && velocity.x > 0)
                {
                    newX = MAX_X - diameter;
                    newVelocityX = -velocity.x;
                }

                if (newY < MIN_Y && velocity.y < 0)
                {
                    newY = MIN_Y;
                    newVelocityY = -velocity.y;
                }
                else if (newY + diameter > MAX_Y && velocity.y > 0)
                {
                    newY = MAX_Y - diameter;
                    newVelocityY = -velocity.y;
                }

                if (newX != position.x || newY != position.y)
                {
                    this.dataLayer.GetLogger().AddLog(ball.UnderneathBall);

                    ball.UnderneathBall.UpdateVelocity(Data.DataAbstractAPI.CreateVector(newVelocityX, newVelocityY));
                }
            }
        }

        private bool CheckCollision(Ball ball1, Ball ball2)
        {
            double dx = ball1.UnderneathBall.Position.x - ball2.UnderneathBall.Position.x;
            double dy = ball1.UnderneathBall.Position.y - ball2.UnderneathBall.Position.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            
            double minDistance = (ball1.UnderneathBall.Diameter + ball2.UnderneathBall.Diameter) / 2 * 1.01;
            
            return distance <= minDistance;
        }

        private void ResolveCollision(Ball ball1, Ball ball2)
        {
            lock (LockObject)
            {
                double dx = ball2.UnderneathBall.Position.x - ball1.UnderneathBall.Position.x;
                double dy = ball2.UnderneathBall.Position.y - ball1.UnderneathBall.Position.y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                
                // Normalizacja wektora normalnego
                double nx = dx / distance;
                double ny = dy / distance;

                // Prędkości względne
                double vx = ball2.UnderneathBall.Velocity.x - ball1.UnderneathBall.Velocity.x;
                double vy = ball2.UnderneathBall.Velocity.y - ball1.UnderneathBall.Velocity.y;
                double relativeVelocity = vx * nx + vy * ny;

                // Jeśli piłki się oddalają, nie ma potrzeby rozwiązywania kolizji
                if (relativeVelocity > 0)
                    return;

                this.dataLayer.GetLogger().AddLog(ball1.UnderneathBall, ball2.UnderneathBall);

                // Dla zderzenia sprężystego, prędkość względna po zderzeniu jest przeciwna
                double impulse = -2.0 * relativeVelocity;
                impulse /= 1 / ball1.Mass + 1 / ball2.Mass;

                // Aktualizacja prędkości
                ball1.UnderneathBall.UpdateVelocity(Data.DataAbstractAPI.CreateVector(
                  ball1.UnderneathBall.Velocity.x - (impulse * nx / ball1.Mass),
                  ball1.UnderneathBall.Velocity.y - (impulse * ny / ball1.Mass)
                ));

                ball2.UnderneathBall.UpdateVelocity(Data.DataAbstractAPI.CreateVector(
                  ball2.UnderneathBall.Velocity.x + (impulse * nx / ball2.Mass),
                  ball2.UnderneathBall.Velocity.y + (impulse * ny / ball2.Mass)
                ));

                // Zapobieganie nakładaniu się piłek
                double overlap = (ball1.UnderneathBall.Diameter + ball2.UnderneathBall.Diameter) / 2 - distance;
                if (overlap > 0)
                {
                    double moveX = nx * overlap * 0.5;
                    double moveY = ny * overlap * 0.5;
                    
                    ball1.UnderneathBall.UpdatePosition(Data.DataAbstractAPI.CreateVector(
                      ball1.UnderneathBall.Position.x - moveX,
                      ball1.UnderneathBall.Position.y - moveY
                    ));
                    
                    ball2.UnderneathBall.UpdatePosition(Data.DataAbstractAPI.CreateVector(
                      ball2.UnderneathBall.Position.x + moveX,
                      ball2.UnderneathBall.Position.y + moveY
                    ));
                }
            }
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