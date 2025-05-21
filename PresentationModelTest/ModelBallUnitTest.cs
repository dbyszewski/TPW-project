using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
    [TestClass]
    public class ModelBallUnitTest
    {
    [TestMethod]
    public void ConstructorTestMethod()
    {
        ModelBall ball = new ModelBall(0.0, 0.0, new BusinessLogicIBallFixture());
        Assert.AreEqual<double>(0.0, ball.Top);
        Assert.AreEqual<double>(0.0, ball.Top);
    }

    [TestMethod]
    public void PositionChangeNotificationTestMethod()
    {
        int notificationCounter = 0;
        ModelBall ball = new ModelBall(0, 0.0, new BusinessLogicIBallFixture());
        ball.PropertyChanged += (sender, args) => notificationCounter++;
        Assert.AreEqual(0, notificationCounter);
        ball.SetLeft(1.0);
        Assert.AreEqual<int>(1, notificationCounter);
        Assert.AreEqual<double>(1.0, ball.Left);
        Assert.AreEqual<double>(0.0, ball.Top);
        ball.SettTop(1.0);
        Assert.AreEqual(2, notificationCounter);
        Assert.AreEqual<double>(1.0, ball.Left);
        Assert.AreEqual<double>(1.0, ball.Top);
    }

    #region testing instrumentation

    private class BusinessLogicPositionFixture : BusinessLogic.IPosition
    {
        public double x { get; init; } = 1.0;
        public double y { get; init; } = 1.0;


        public BusinessLogicPositionFixture(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    private class BusinessLogicIBallFixture : BusinessLogic.IBall
    {
        public event EventHandler<IPosition>? NewPositionNotification;
        public double Diameter { get; } = 30.0;
        public double Mass { get; } = 1.0;
        public IPosition Position { get; } = new BusinessLogicPositionFixture(0, 0);

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    #endregion testing instrumentation
    }
}