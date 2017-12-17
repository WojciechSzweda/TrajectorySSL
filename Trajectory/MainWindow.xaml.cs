using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Trajectory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetArcSettings();

        }
        CanvasPoint StartPoint = new CanvasPoint();
        CanvasPoint EnemyPoint = new CanvasPoint();
        ArcSettings Arc;
        void DrawLine(double x1, double y1, double x2, double y2, Color color)
        {
            var line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            //line.Stroke = Brushes.White;
            var brush = new SolidColorBrush();
            brush.Color = color;
            line.Stroke = brush;
            line.StrokeThickness = 3;
            Screen.Children.Add(line);

        }

        private void Screen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                StartMark(e.GetPosition(this));
            else if (e.ChangedButton == MouseButton.Right)
                EnemyMark(e.GetPosition(this));

        }

        void EnemyMark(Point point)
        {
            ClearElllipse("Enemy");
            MakeEllipseAtPoint(point, Color.FromRgb(255, 0, 0), "Enemy");
            EnemyPoint = new CanvasPoint(point);

        }

        void GetArcSettings()
        {
            Arc = new ArcSettings(sliderAngle.Value, sliderPower.Value, sliderStepSize.Value, double.Parse(tbGravity.Text), cbLeft.IsChecked == true ? -1 : 1);
        }

        void StartMark(Point point)
        {
            ClearElllipse("Ally");
            MakeEllipseAtPoint(point, Color.FromRgb(0, 255, 0), "Ally");
            StartPoint = new CanvasPoint(point);
            MakeArc();
        }

        void MakeEllipseAtPoint(Point point, Color color, string name)
        {
            var Ellipse = new Ellipse();
            Ellipse.Height = 10;
            Ellipse.Width = 10;
            Ellipse.Name = name;
            var brush = new SolidColorBrush();
            brush.Color = color;
            Ellipse.Fill = brush;
            Screen.Children.Add(Ellipse);
            Canvas.SetLeft(Ellipse, point.X - Ellipse.Width / 2);
            Canvas.SetTop(Ellipse, point.Y - Ellipse.Height / 2);
        }

        private void btnFire_Click(object sender, RoutedEventArgs e)
        {
            GetArcSettings();
            MakeArc();
        }

        bool hasNewArcSettings()
        {
            if (Arc.Angle != sliderAngle.Value)
                return true;
            if (Arc.Power != sliderPower.Value)
                return true;
            if (Arc.StepSize != sliderStepSize.Value)
                return true;
            return false;
        }

        void MakeArc()
        {
            if (!hasNewArcSettings() || !StartPoint.Set)
                return;
            GetArcSettings();
            ClearArcs();

            double y0 = 0;
            double x0 = 0;
            double prevX = x0, prevY = y0;

            for (int i = 0; i < Arc.Steps; i++)
            {
                double x1 = prevX + Arc.StepSize;
                double y1 = y0 + x1 * Math.Tan(Arc.Angle) - (Arc.Gravity * x1 * x1 / (2 * Math.Pow((Arc.Power * Math.Cos(Arc.Angle)), 2)));
                if (i % 2 == 0)
                    DrawLine(StartPoint.Point.X + Arc.Dir * prevX, StartPoint.Point.Y - prevY, StartPoint.Point.X + Arc.Dir * x1, StartPoint.Point.Y - y1, (Color)clpTracerColor.SelectedColor);
                prevX = x1;
                prevY = y1;
            }

        }

        private void tbClear_Click(object sender, RoutedEventArgs e)
        {
            ClearCanvas();
        }

        void ClearCanvas()
        {
            ClearArcs();
            Screen.Children.OfType<Ellipse>().ToList().ForEach(x => Screen.Children.Remove(x));
        }
        void ClearElllipse(string name)
        {
            Screen.Children.OfType<Ellipse>().Where(x => x.Name == name).ToList().ForEach(x => Screen.Children.Remove(x));
        }

        void ClearArcs()
        {
            Screen.Children.OfType<Line>().ToList().ForEach(x => Screen.Children.Remove(x));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCalcAngle_Click(object sender, RoutedEventArgs e)
        {
            CalcAngle();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int dir = cbLeft.IsChecked == true ? -1 : 1;
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.Left)
                sliderAngle.Value += sliderAngle.TickFrequency * dir;
            else if (e.Key == Key.Right)
                sliderAngle.Value -= sliderAngle.TickFrequency * dir;
            else if (e.Key == Key.Up)
                sliderPower.Value += sliderPower.TickFrequency;
            else if (e.Key == Key.Down)
                sliderPower.Value -= sliderPower.TickFrequency;
            else if (e.Key == Key.Space)
                MakeArc();
            else if (e.Key == Key.LeftCtrl)
                cbLeft.IsChecked = !cbLeft.IsChecked;
        }

        void RefreshArc()
        {
            ClearArcs();
            MakeArc();
        }

        private void SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Arc == null || !StartPoint.Set)
                return;

            RefreshArc();
        }

        private void cbLeft_Checked(object sender, RoutedEventArgs e)
        {
            if (Arc == null || !StartPoint.Set)
                return;

            RefreshArc();
        }

        void CalcAngle()
        {
            var point = new Point(EnemyPoint.Point.X - StartPoint.Point.X, StartPoint.Point.Y - EnemyPoint.Point.Y);
            var preSqrt = Math.Pow(Arc.Power, 4) - Arc.Gravity * ((Arc.Gravity * point.X * point.X) + (2 * point.Y * Arc.Power * Arc.Power));
            if (preSqrt < 0)
            {
                tbCalcAngle.Text = "more power";
                return;
            }
            var sqrt = Math.Sqrt(preSqrt);
            var radians = Math.Atan(((Arc.Power * Arc.Power) + Arc.Dir * sqrt) / (Arc.Gravity * point.X));
            var angle = radians * (180 / Math.PI);

            tbCalcAngle.Text = Math.Round(angle).ToString();
        }

        private void clpTracerColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (Arc == null || !StartPoint.Set)
                return;
            RefreshArc();
        }
    }
}
