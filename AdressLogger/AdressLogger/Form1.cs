using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using System.Diagnostics;
using System.IO;

namespace AdressLogger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DeepPointer[] ptrs = {
            new DeepPointer("",0x520,new int[] {0x70,0x120}),
            new DeepPointer("",0x520,new int[] {0x70,0x128}),
            new DeepPointer("",0x520,new int[] {0x70,0x130}),
            new DeepPointer("",0x0714F830,new int[] {0x20})
        };
        Process _game = null;
        string saveFolder = @"C:\OBS\LR-repo\CE-tables\";
        public struct myPosData
        {
            public myPosData(double x = Double.NaN, double y = Double.NaN, double z = Double.NaN)
            {
                Time = DateTime.Now;
                s = 0;
                PosX = x;
                PosY = y;
                PosZ = z;
            }
            public myPosData(DateTime time, double x = 0, double y = 0, double z = 0)
            {
                Time = time;
                s = 0;
                PosX = x;
                PosY = y;
                PosZ = z;
            }
            public DateTime Time;
            public double s;
            public double PosX;
            public double PosY;
            public double PosZ;
            public bool valid()
            {
                return PosX != 0 && PosY != 0;
            }
            public string ToString(DateTime dateRef)
            {
                return ToString(dateRef.Ticks);
            }

            public string ToString(long timeRef = 0)
            {
                double elapsedSecs = (Time.Ticks - timeRef) / ((double)System.TimeSpan.TicksPerSecond);
                return elapsedSecs.ToString("0.000") + "," + s.ToString("0.00") + "," + PosX.ToString("0.00") + "," + PosY.ToString("0.00") + "," + PosZ.ToString("0.00");
            }
        }
        List<myPosData> myData = new List<myPosData>();
        myPosData posCmp = new myPosData();
        double totalS = 0;

        enum loggingStates
        {
            Idle,
            Armed,
            Logging
        }

        private bool loggingInProg()
        {
            return logState == loggingStates.Logging;
        }
        loggingStates logState = loggingStates.Idle;


        public double calcLatVel(myPosData prev, myPosData cur)
        {
            double value = Double.NaN;
            if (prev.valid() && cur.valid())
            {
                double deltaT = cur.Time.Ticks - prev.Time.Ticks;
                deltaT /= ((double)System.TimeSpan.TicksPerSecond);
                double deltaS = Math.Sqrt(calcLatDistSq(prev, cur)); ;
                if (deltaT > 0)
                {
                    value = deltaS / deltaT;
                }
            }
            return value;
        }

        public double calcLatDistSq(myPosData prev, myPosData cur)
        {
            double value = Double.NaN;
            if (prev.valid() && cur.valid())
            {
                value = (cur.PosX - prev.PosX) * (cur.PosX - prev.PosX) + (cur.PosY - prev.PosY) * (cur.PosY - prev.PosY);
            }
            return value;
        }

        private void ArmLogging()
        {
            logState = loggingStates.Armed;
            labelState.Text = "State: Armed";
            myData.Clear();
            myData.Capacity = (int)((nUD_Time.Value + 1) * 1000 / timerLogging.Interval);
            timerLogging.Interval = (int)((nUD_Time.Value) * 1000);
            timerLogging.Stop();
        }

        private void StartLogging()
        {
            logState = loggingStates.Logging;
            labelState.Text = "State: In Progress";
            timerLogging.Start();
        }

        private void AbortLogging()
        {
            timerLogging.Stop();
            logState = loggingStates.Idle;
            labelState.Text = "State: Idle";
            myData.Clear();
            posCmp = new myPosData();
        }

        private void CompleteLogging()
        {
            timerLogging.Stop();
            timerMemory.Stop();
            SaveBuffer();
            AbortLogging();
            timerMemory.Start();
        }

        public void SaveBuffer()
        {
            string fileName = saveFolder + posCmp.Time.ToString("yyyy-MM-ddTHH-mm-ss") + ".csv";
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine("t,s,X,Y,Z");
            for (int i = 0; i < myData.Count; ++i)
            {
                sw.WriteLine(myData[i].ToString(posCmp.Time));
            }
            sw.Close();
        }

        public void UpdateGameActive()
        {
            double[] values = new double[3];
            for (int i = 0; i < 3; ++i)
            {
                values[i] = ptrs[i].Deref<double>(_game, Double.NaN);
            }
            labelPos.Text = "Position:\n" + values[0].ToString("0.##") +
                "\n" + values[1].ToString("0.##") +
                "\n" + values[2].ToString("0.##");
            myPosData posCur = new myPosData(values[0], values[1], values[2]);

            switch (logState)
            {
                case loggingStates.Idle:
                    break;
                case loggingStates.Armed:
                    if (!posCmp.valid())
                    {
                        posCmp = posCur;
                    }
                    if (calcLatDistSq(posCmp, posCur) > (0.01))
                    {
                        StartLogging();
                        myData.Add(posCur);
                        posCmp = posCur;
                        totalS = 0;
                    }
                    break;
                case loggingStates.Logging:
                    double dT = (posCur.Time.Ticks - posCmp.Time.Ticks) / ((double)System.TimeSpan.TicksPerSecond);
                    double dS = Math.Sqrt(calcLatDistSq(myData[myData.Count - 1], posCur));
                    totalS += dS;
                    posCur.s = totalS;
                    double vel = totalS / dT;
                    myData.Add(posCur);
                    labelVel.Text = "Velocity-lat:\n" + vel.ToString("0.##");
                    bool gamePaused = (ptrs[3].Deref<UInt32>(_game, 0) > 0);
                    if (gamePaused)
                    {
                        CompleteLogging();
                    }
                    break;
                default:
                    break;
            }
        }

        public void UpdateGame()
        {
            if (_game == null)
            {
                Process myProc = Process.GetProcessesByName("HorizonZeroDawn").OrderByDescending(x => x.StartTime)
                    .FirstOrDefault(x => !x.HasExited);
                if (myProc == null)
                    return;
                _game = myProc;
                logState = loggingStates.Idle;
                labelState.Text = "State: Idle";
            }
            else if (_game.HasExited)
            {
                _game = null;
                labelState.Text = "State: Disconnected";
                if (loggingInProg())
                {
                    AbortLogging();
                }
            }
            else
            {
                UpdateGameActive();
            }
        }

        private void timerMemory_Tick(object sender, EventArgs e)
        {
            UpdateGame();
        }

        private void timerLogging_Tick(object sender, EventArgs e)
        {
            CompleteLogging();
        }

        private void buttonLog_Click(object sender, EventArgs e)
        {
            ArmLogging();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            AbortLogging();
        }
    }
}
