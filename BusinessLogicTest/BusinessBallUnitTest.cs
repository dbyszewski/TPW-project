namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void MoveTestMethod()
        {
            DataBallFixture dataBallFixture = new DataBallFixture();
            Ball newInstance = new(dataBallFixture);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); Assert.IsNotNull(position); numberOfCallBackCalled++; };
            dataBallFixture.Move();
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
        }

        #region testing instrumentation

        private class DataBallFixture : Data.IBall
        {
            public event EventHandler<Data.IVector>? NewPositionNotification;
            public Data.IVector Velocity { get; set; } = new VectorFixture(0, 0);
            public double Diameter { get; } = 30.0;
            public double Mass { get; } = 1.0;
            public Data.IVector Position { get; } = new VectorFixture(0, 0);

            public void UpdatePosition(Data.IVector newPosition)
            {
                
            }

            public void UpdateVelocity(Data.IVector newVelocity)
            {
                Velocity = newVelocity;
            }

            internal void Move()
            {
                NewPositionNotification?.Invoke(this, new VectorFixture(0.0, 0.0));
            }
        }

        private class VectorFixture : Data.IVector
        {
            internal VectorFixture(double X, double Y)
            {
                x = X; y = Y;
            }

            public double x { get; init; }
            public double y { get; init; }
        }

        #endregion testing instrumentation
    }
}