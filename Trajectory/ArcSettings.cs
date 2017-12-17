using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Trajectory
{
    public class ArcSettings
    {
        public double Angle { get; set; }
        public double Power { get; set; }
        public int StepSize { get; set; }
        public int Steps { get; set; }
        public double Gravity { get; set; }
        public int Dir { get; set; }

        public ArcSettings(double angle, double power, double stepsize, double gravity, int dir)
        {
            Angle = angle * (Math.PI / 180);
            Power = power;
            StepSize = (int)stepsize;
            Steps = 2000 / (int)stepsize; //TODO: Optimise
            Gravity = gravity;
            Dir = dir;
        }
    }
}
