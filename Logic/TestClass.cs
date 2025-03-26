using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TPW.Data;

namespace TPW.Logic
{
    public class TestClass
    {
        public TestClass()
        {
            Ball ball = new Ball(0, 0, 10);
            Console.WriteLine("Ball at ({0}, {1}) with radius {2}", ball.X, ball.Y, ball.Radius);
        }
    }
}
