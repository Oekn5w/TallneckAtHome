//#define HFW

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LiveSplit.ComponentUtil;
using System.Diagnostics;
using System.IO;


namespace AdressLogger
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        struct GameData
        {
            public string executableName;
            public DeepPointer[] ptr;
        };

        static GameData[] gameData = new GameData[4];

        public Form1()
        {
            InitializeComponent();
            myDataPrevBuf.Capacity = 500 / timerMemory.Interval;
            for (int i = 0; i < myDataPrevBuf.Capacity; ++i)
            {
                myDataPrevBuf.Add(new myDataDef());
            }
            gameData[0].executableName = "HorizonZeroDawn";
            gameData[0].ptr = new DeepPointer[]
            {
                new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x120}),
                new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x128}),
                new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x130}),
                //new DeepPointer("", 0x0714F830, new int[] {0x160}), // time played
                new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x1F0,0x60}),
                new DeepPointer("", 0x0714F830, new int[] {0x20}),
                new DeepPointer("", 0x0714F830, new int[] {0x4B4}),
                new DeepPointer("", 0x0714F830, new int[] {0x158}), // time factor
                new DeepPointer("", 0x0714F830), // world-active
                new DeepPointer("", 0x07152640) // tabbed out
            };

            // GoG
            gameData[1].executableName = "HorizonZeroDawn";
            gameData[1].ptr = new DeepPointer[]
            {
                new DeepPointer("", 0x07146f88, new int[] {0x30,0x120}),
                new DeepPointer("", 0x07146f88, new int[] {0x30,0x128}),
                new DeepPointer("", 0x07146f88, new int[] {0x30,0x130}),
                //new DeepPointer("", 0x0714C728, new int[] {0x160}), // time played
                new DeepPointer("", 0x07146f88, new int[] {0x30,0x1F0,0x60}),
                new DeepPointer("", 0x0714C728, new int[] {0x20}),
                new DeepPointer("", 0x0714C728, new int[] {0x4B4}),
                new DeepPointer("", 0x0714C728, new int[] {0x158}), // time factor
                new DeepPointer("", 0x0714C728), // world-active
                new DeepPointer("", 0x0714c800) // tabbed out
            };

            gameData[2].executableName = "HorizonZeroDawnRemastered";
            gameData[2].ptr = new DeepPointer[]
            {
                new DeepPointer("", 0x099A9A38 - 0x0E38, new int[] {0xA0,0,0x28,0x150}),
                new DeepPointer("", 0x099A9A38 - 0x0E38, new int[] {0xA0,0,0x28,0x158}),
                new DeepPointer("", 0x099A9A38 - 0x0E38, new int[] {0xA0,0,0x28,0x160}),
                //new DeepPointer("", 0x099A9A38, new int[] {0x160}), // time played
                new DeepPointer("", 0x099A9A38 - 0x0E38, new int[] {0xA0,0,0x28,0x208,0x60}),
                new DeepPointer("", 0x099A9A38, new int[] {0x20}),
                new DeepPointer("", 0x099A9A38, new int[] {0x4DC}),
                new DeepPointer("", 0x099A9A38, new int[] {0x158}), // time factor
                new DeepPointer("", 0x099A9A38), // N/A
                new DeepPointer("", 0x099A9A38, new int[] {0x172}) // tabbed in
            };
            gameData[3].executableName = "HorizonForbiddenWest";
            gameData[3].ptr = new DeepPointer[]
            {
                new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xD8}),
                new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xE0}),
                new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xE8}),
                new DeepPointer("", 0x8982DA0, new int[] {0x1C10, 0x0, 0x10, 0xD0, 0x70}),
                new DeepPointer("", 0x08983150, new int[] {0x20}),
                new DeepPointer("", 0x08983150, new int[] {0x4B4}),
                new DeepPointer("", 0x08983150, new int[] {0x158}), // time factor
                new DeepPointer("", 0x08983150), // N/A
                new DeepPointer("", 0x08983150) // N/A
            };
            comboBox1.SelectedIndex = 0;
        }

        static Process _game = null;
        string saveFolder = @"C:\Speedrunning-dev\AS-repo\CE-tables\";
        List<myDataDef> myDataPrevBuf = new List<myDataDef>();
        List<myDataDef> myData = new List<myDataDef>();
        string[] tempLbls = new string[3];
        static int idx = 0;

        public struct myDataDef
        {
            public DateTime Time;
            public double PosX;
            public double PosY;
            public double PosZ;
            public double Velocity;
            public double Heading;
            public byte Invulnerable;
            public uint Pause;
            public uint Loading;
            public byte TimeFac;
            public ulong WorldActive;
            public byte WindowActive;

            public myDataDef(ref Process _game)
            {
                Time = DateTime.Now;
                PosX = Double.NaN;
                PosY = Double.NaN;
                PosZ = Double.NaN;
                Velocity = Double.NaN;
                Heading = Double.NaN;
                Invulnerable = byte.MaxValue;
                Pause = uint.MaxValue;
                Loading = uint.MaxValue;
                TimeFac = byte.MaxValue;
                WorldActive = 0;
                WindowActive = byte.MaxValue;
                if (_game != null)
                {
                    PosX = gameData[idx].ptr[0].Deref<Double>(_game, Double.NaN);
                    PosY = gameData[idx].ptr[1].Deref<Double>(_game, Double.NaN);
                    PosZ = gameData[idx].ptr[2].Deref<Double>(_game, Double.NaN);
                    Invulnerable = gameData[idx].ptr[3].Deref<byte>(_game, byte.MaxValue);
                    Pause = gameData[idx].ptr[4].Deref<uint>(_game, uint.MaxValue);
                    Loading = gameData[idx].ptr[5].Deref<uint>(_game, uint.MaxValue);
                    TimeFac = gameData[idx].ptr[6].Deref<byte>(_game, byte.MaxValue);
                    WorldActive = gameData[idx].ptr[7].Deref<ulong>(_game, ulong.MaxValue);
                    WindowActive = gameData[idx].ptr[8].Deref<byte>(_game, byte.MaxValue);
                }
            }

            public void calc(myDataDef dataPrev)
            {
                if (valid() && dataPrev.valid())
                {
                    double deltaT = (Time.Ticks - dataPrev.Time.Ticks) / ((double)System.TimeSpan.TicksPerSecond);
                    double dX = PosX - dataPrev.PosX;
                    double dY = PosY - dataPrev.PosY;
                    double deltaS = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2) + Math.Pow(PosZ - dataPrev.PosZ, 2));
                    Velocity = deltaS / deltaT;
                    if (deltaS > 0.05)
                    {
                        Heading = (Math.Atan2(dX, dY) * 180.0 / Math.PI); // This is shifted for geographical heading
                        if (Heading < 0)
                        {
                            Heading += 360.0;
                        }
                    }
                }
            }

            public bool valid()
            {
                return (!Double.IsNaN(PosX) && !Double.IsNaN(PosY) && !Double.IsNaN(PosZ) && Loading == 0 && Pause == 0);
            }
            public string ToString(DateTime dateRef)
            {
                return ToString(dateRef.Ticks);
            }

            public static string LogHeaderFile()
            {
                return "t,t_rec,X,Y,Z,vel,head,invul,pause,load,timeMul,worldActive,windowActive";
            }

            public string ToString(long timeRef = 0)
            {
                // t,t_rec,X,Y,Z,vel,head,invul,pause,load,TF,worldA,WindowActive
                double elapsedSecs = (Time.Ticks - timeRef) / ((double)System.TimeSpan.TicksPerSecond);
                return Time.ToString("yyyy-MM-ddTHH-mm-ss.FFF") + "," + elapsedSecs.ToString("0.000") + "," + PosX.ToString("0.000") + "," + PosY.ToString("0.000") + "," + PosZ.ToString("0.000") + "," + Velocity.ToString("0.000") + "," + Heading.ToString("000") + ",0x" + Invulnerable.ToString("X02") + ",0x" + Pause.ToString("X08") + ",0x" + Loading.ToString("X08") + ",0x" + TimeFac.ToString("X02") + ",0x" + ((byte)((WorldActive>0)?1:0)).ToString("X02") + ",0x" + WindowActive.ToString("X02");
            }

            public void WriteToLabels(out string textPos, out string textInfo, out string textStates)
            {
                textPos = "Position:\n" + PosX.ToString("0.00") +
                    "\n" + PosY.ToString("0.00") +
                    "\n" + PosZ.ToString("0.00");
                textInfo = "Vel:\n" + Velocity.ToString("0.0") +
                    "\nHead:\n" + Heading.ToString("000");
                textStates = "States:\nPause " + Pause.ToString("X08") +
                    "\nLoads " + Loading.ToString("X08") +
                    "\nInv  TF   Wrld WinA" +
                    "\n0x" + Invulnerable.ToString("X02") +
                    " 0x" + TimeFac.ToString("X02") +
                    " 0x" + ((byte)((WorldActive > 0) ? 1 : 0)).ToString("X02") +
                    " 0x" + WindowActive.ToString("X02");
            }
        }

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

        private void ArmLogging()
        {
            logState = loggingStates.Armed;
            labelState.Text = "State: Armed";
            myData.Clear();
            myData.Capacity = (int)(((nUD_Time.Value + 1) * 1000) / timerMemory.Interval);
            timerLogging.Interval = (int)((nUD_Time.Value) * 1000);
            timerLogging.Stop();
        }

        private void StartLogging()
        {
            buttonLogStuff.Text = "Stop logging";
            logState = loggingStates.Logging;
            labelState.Text = "State: In Progress";
            timerLogging.Start();
        }

        private void StopLogging()
        {
            timerLogging.Stop();
            logState = loggingStates.Idle;
            labelState.Text = "State: Idle";
            buttonLogStuff.Text = "Arm logging";
            myData.Clear();
        }

        private void CompleteLogging()
        {
            timerLogging.Stop();
            timerMemory.Stop();
            SaveBuffer();
            StopLogging();
            timerMemory.Start();
        }

        public void SaveBuffer()
        {
            if (myData.Count == 0)
            {
                return;
            }
            if (!System.IO.Directory.Exists(saveFolder))
            {
                saveFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\";
            }
            string fileName = saveFolder + myData[0].Time.ToString("yyyy-MM-ddTHH-mm-ss") + ".csv";
            StreamWriter sw = new StreamWriter(fileName);
            sw.WriteLine(myDataDef.LogHeaderFile());
            for (int i = 0; i < myData.Count; ++i)
            {
                sw.WriteLine(myData[i].ToString(myData[0].Time));
            }
            sw.Close();
        }

        public void UpdateGameActive()
        {
            myDataDef myDataCur = new myDataDef(ref _game);
            myDataCur.calc(myDataPrevBuf[myDataPrevBuf.Count - 1]);
            for (int i = myDataPrevBuf.Count - 1; i > 0; --i)
            {
                myDataPrevBuf[i] = myDataPrevBuf[i - 1];
            }
            myDataPrevBuf[0] = myDataCur;
            myDataCur.WriteToLabels(out tempLbls[0], out tempLbls[1], out tempLbls[2]);
            labelPos.Text = tempLbls[0];
            labelInfo.Text = tempLbls[1];
            labelStates.Text = tempLbls[2];

            switch (logState)
            {
                case loggingStates.Logging:
                    myData.Add(myDataCur);
                    break;
                case loggingStates.Armed:
                    var activatedHandle = GetForegroundWindow();
                    if (activatedHandle == IntPtr.Zero)
                    {
                        break;       // No window is currently activated
                    }
                    int activeProcID;
                    GetWindowThreadProcessId(activatedHandle, out activeProcID);
                    if (_game.Id == activeProcID)
                    {
                        myData.Add(myDataCur);
                        StartLogging();
                    }
                    break;
                default:
                    break;
            }
        }

        public void UpdateGame()
        {
            try
            {
                if (_game == null)
                {
                    Process myProc = null;
                    try
                    {
                        myProc = Process.GetProcessesByName(gameData[idx].executableName).OrderByDescending(x => x.StartTime)
                        .FirstOrDefault(x => !x.HasExited);
                    }
                    catch (Exception e)
                    {
                        myProc = null;
                    }
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
                        StopLogging();
                    }
                }
                else
                {
                    UpdateGameActive();
                }
            }
            catch (Exception e)
            {
                _game = null;
                labelState.Text = "State: Disconnected";
                if (loggingInProg())
                {
                    StopLogging();
                }
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

        private void buttonLogStuff_Click(object sender, EventArgs e)
        {
            switch (logState)
            {
                case loggingStates.Idle:
                case loggingStates.Armed:
                    ArmLogging();
                    break;
                case loggingStates.Logging:
                    CompleteLogging();
                    break;
                default:
                    break;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            idx = comboBox1.SelectedIndex;
            _game = null;
            labelState.Text = "State: Disconnected";
            if (loggingInProg())
            {
                StopLogging();
            }
        }
    }
}
