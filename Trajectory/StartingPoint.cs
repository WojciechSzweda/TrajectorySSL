using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Trajectory
{
    public class CanvasPoint
    {
        public bool Set { get; set; }
        public Point Point { get; set; }

        public CanvasPoint()
        {
            Set = false;
            Point = new Point();
        }
        public CanvasPoint(Point point)
        {
            Set = true;
            Point = point;
        }
    }
}
