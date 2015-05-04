using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace _2AxisJoyStickMapper
{
    public partial class Form1 : Form
    {
        private List<IDataPoint> JoystickPoints;

        private LineSeries points;

        private PlotModel pm;

        private BackgroundWorker background;

        public Form1()
        {
            InitializeComponent();

            JoystickPoints = new List<IDataPoint>();
            points = new LineSeries("JoyStick Points");
            points.Points = JoystickPoints;

            JoystickPoints.Capacity = 100;

            pm =  new PlotModel("2-Axis Joystick", "Plots coordinates of the joystick position")
            {
                PlotType = PlotType.Cartesian,
                Background = OxyColors.White
            };

            pm.Axes.Add(new LinearAxis(AxisPosition.Bottom, 0, 1023, 100, 10, "X"));
            pm.Axes.Add(new LinearAxis(AxisPosition.Left, 0, 1023, 100, 10, "Y"));

            pm.Series.Add(points);
            plot1.Model = pm;

            //ArduinoPort.NewLine = "\n";
            ArduinoPort.Open();

            //background.RunWorkerCompleted += background_RunWorkerCompleted;
        }

        void background_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ArduinoPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (e.EventType == System.IO.Ports.SerialData.Chars)
            {
                string pt = ArduinoPort.ReadLine();

                if (pt.Contains(',') && !pt.Contains('?') && pt.Length <= 12)
                {
                    string x = pt.Substring(0, pt.IndexOf(',')).Trim();
                    string y = pt.Substring(pt.IndexOf(',') + 2).Trim();
                    //if (JoystickPoints.Last().X.ToString() != x && JoystickPoints.Last().Y.ToString() != y)
                    {
                        JoystickPoints.Add(new DataPoint(Convert.ToDouble(x), Convert.ToDouble(y)));
                    }
                    
                    if (JoystickPoints.Count == 100)
                    {
                        JoystickPoints.RemoveAt(0);
                    }
                    this.Invoke((Action)delegate { pm.RefreshPlot(true); });
                    this.Invoke((Action)delegate { plot1.Refresh(); });
                }
            }
        }
    }
}
