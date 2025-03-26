using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPW.Data
{
    public class Ball
    {
        public Ball(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Radius { get; private set; }
    }
}
