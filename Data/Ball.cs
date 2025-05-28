namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        internal Ball(Vector initialPosition, Vector initialVelocity, double diameter, double mass)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            Diameter = diameter;
            Mass = mass;
        }

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        public double Diameter { get; private init; }

        public double Mass { get; private init; }

        public IVector Position { get; private set; }

        public void UpdateVelocity(IVector newVelocity)
        {
            Velocity = newVelocity;
        }

        public void UpdatePosition(IVector newPosition)
        {
            Position = newPosition;
        }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            Position = new Vector(Position.x + delta.x, Position.y + delta.y);
            RaiseNewPositionChangeNotification();
        }
    }
}
