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

namespace RunLogger
{
    public partial class RunLogger : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        struct GameDataEntry
        {
            public string name;
            public DeepPointer ptr;
            public string type;
            public bool toLog;
            public GameDataEntry(string n, DeepPointer p, string t, bool l)
            {
                name = n;
                ptr = p;
                type = t;
                toLog = l;
            }
        }
        Dictionary<string, string> floatingFormats = new Dictionary<string, string>()
        {
            { "0","0"},
            { "1","0.0"},
            { "2","0.00"},
            { "3","0.000"},
            { "4","0.0000"}
        };

        struct GameData
        {
            public string executableName;
            public GameDataEntry[] gamePtrDef;
        };

        static GameData[] gameData;
        static Process gameProcess = null;
        List<string> dataBuf;
        string dataBufFileName;

        bool loggingRequest = false;
        bool loggingInProg = false;
#if DEBUG
        const int BUFFER_CAPA = 120;
#else
        const int BUFFER_CAPA = 7200;
#endif

        public RunLogger()
        {
            InitializeComponent();
            gameData = new GameData[2];
            gameData[0].executableName = "HorizonZeroDawn";
            gameData[0].gamePtrDef = new GameDataEntry[]
            {
                new GameDataEntry("posX",new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x120}),"d2",true),
                new GameDataEntry("posY",new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x128}),"d2",true),
                new GameDataEntry("posZ",new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x130}),"d2",true),
                new GameDataEntry("igt",new DeepPointer("", 0x0714F830, new int[] {0x160}),"d2",true),
                new GameDataEntry("load",new DeepPointer("", 0x0714F830, new int[] {0x4B4}),"inz",true),
                new GameDataEntry("paused",new DeepPointer("", 0x0714F830, new int[] {0x20}),"inz",true),
                new GameDataEntry("invul",new DeepPointer("", 0x0714cdc8, new int[] {0x30, 0x1F0, 0x60}),"bnz",true),
                new GameDataEntry("mounted",new DeepPointer("", 0x0714cdc8, new int[] {0x30, 0x1C8}),"lnz",true)
            };
            gameData[1].executableName = "HorizonForbiddenWest";
            gameData[1].gamePtrDef = new GameDataEntry[]
            {
                new GameDataEntry("posX",new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xD8}),"d2",true),
                new GameDataEntry("posY",new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xE0}),"d2",true),
                new GameDataEntry("posZ",new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xE8}),"d2",true),
                new GameDataEntry("igt",new DeepPointer("", 0x08983150, new int[] {0x160}),"d2",true),
                new GameDataEntry("load",new DeepPointer("", 0x08983150, new int[] {0x4B4}),"inz",true),
                new GameDataEntry("paused",new DeepPointer("", 0x08983150, new int[] {0x20}),"inz",true),
                new GameDataEntry("invul",new DeepPointer("", 0x8982DA0, new int[] {0x1C10, 0x0, 0x10, 0xD0, 0x70}),"bnz",true),
                new GameDataEntry("mountDestructability",new DeepPointer("", 0x08982DA0, new int[] {0x1C10, 0x0, 0x10, 0x80, 0xD0}),"lx",true)
            };
            dataBuf = new List<string>();
            dataBuf.Capacity = BUFFER_CAPA;
            if (!Directory.Exists(tbLogFolder.Text))
            {
                tbLogFolder.Text = "";
            }
            cbGame.SelectedIndex = 0;
            UpdateButtonState();
            UpdateStateLbl();
        }

        private void btnLogCtl_Click(object sender, EventArgs e)
        {
            loggingRequest = !loggingRequest;
            UpdateButtonState();
            UpdateStateLbl();
        }

        private void btnFolderSelect_Click(object sender, EventArgs e)
        {
            folderDialogue.SelectedPath = tbLogFolder.Text;
            var diaRes = folderDialogue.ShowDialog();
            if (diaRes == DialogResult.OK)
            {
                tbLogFolder.Text = folderDialogue.SelectedPath;
            }
        }

        private void timerLogging_Tick(object sender, EventArgs e)
        {
            UpdateGameProcess();
            loggingTick();
            UpdateStateLbl();
        }

        private void loggingTick()
        {
            bool prevLog = loggingInProg;
            loggingInProg = loggingRequest && (gameProcess != null);
            string log = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss.FFF");
            if (loggingInProg)
            {
                int idx = cbGame.SelectedIndex;
                int nData = gameData[idx].gamePtrDef.Length;
                string temp;
                for (int i = 0; i < nData; i++)
                {
                    if (!gameData[idx].gamePtrDef[i].toLog)
                        continue;
                    try
                    {
                        temp = getDataEntry(gameData[idx].gamePtrDef[i]);
                    }
                    catch (Win32Exception e)
                    {
                        gameProcess = null;
                        break;
                    }
                    log += "," + temp;
                }
                loggingInProg = (gameProcess != null);
            }
            if (!loggingInProg && prevLog)
            {
                WriteOutBuffer();
            }
            if (loggingInProg)
            {
                dataBuf.Add(log);
                if (dataBuf.Count() == 1)
                {
                    dataBufFileName = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
                }
                if (dataBuf.Count() == BUFFER_CAPA)
                {
                    WriteOutBuffer();
                }
                lblBuffer.Text = dataBuf.Count().ToString();
            }
        }

        string getDataEntry(GameDataEntry entry)
        {
            if (entry.type.StartsWith("d"))
            {
                double value = entry.ptr.Deref<double>(gameProcess, Double.NaN);
                return value.ToString(floatingFormats[entry.type.Substring(1)]);
            }
            else if (entry.type.StartsWith("f"))
            {
                float value = entry.ptr.Deref<float>(gameProcess, float.NaN);
                return value.ToString(floatingFormats[entry.type.Substring(1)]);
            }
            else if (entry.type.StartsWith("b"))
            {
                byte value;
                if (entry.type.EndsWith("nz"))
                {
                    value = entry.ptr.Deref<byte>(gameProcess, 0);
                    return (value > 0).ToString();
                }
                else
                {
                    value = entry.ptr.Deref<byte>(gameProcess, byte.MaxValue);
                    if (entry.type.EndsWith("x"))
                        return "0x" + value.ToString("X02");
                    else
                        return value.ToString();
                }
            }
            else if (entry.type.StartsWith("i"))
            {
                int value;
                if (entry.type.EndsWith("nz"))
                {
                    value = entry.ptr.Deref<int>(gameProcess, 0);
                    return (value > 0).ToString();
                }
                else
                {
                    value = entry.ptr.Deref<int>(gameProcess, int.MaxValue);
                    if (entry.type.EndsWith("x"))
                        return "0x" + value.ToString("X08");
                    else
                        return value.ToString();
                }
            }
            else if (entry.type.StartsWith("l"))
            {
                long value;
                if (entry.type.EndsWith("nz"))
                {
                    value = entry.ptr.Deref<long>(gameProcess, 0);
                    return (value > 0).ToString();
                }
                else
                {
                    value = entry.ptr.Deref<long>(gameProcess, long.MaxValue);
                    if (entry.type.EndsWith("x"))
                        return "0x" + value.ToString("X016");
                    else
                        return value.ToString();
                }
            }
            throw new Exception("Unrecognized pointer type");
        }

        private void WriteOutBuffer()
        {
            string fileName = tbLogFolder.Text + "\\" + dataBufFileName + ".csv";
            int file_i = 1;
            while (File.Exists(fileName))
            {
                fileName = tbLogFolder.Text + "\\" + dataBufFileName + "-" + file_i++.ToString() + ".csv";
            }
            StreamWriter sw = new StreamWriter(fileName);
            string header = "Time";
            int idx = cbGame.SelectedIndex;
            int nData = gameData[idx].gamePtrDef.Length;
            for (int i = 0; i < nData; i++)
            {
                if (!gameData[idx].gamePtrDef[i].toLog)
                    continue;
                header += "," + gameData[idx].gamePtrDef[i].name;
            }
            sw.WriteLine(header);
            for (int i = 0; i < dataBuf.Count(); i++)
            {
                sw.WriteLine(dataBuf[i]);
            }
            sw.Close();
            dataBuf.Clear();
            lblWriting.Text = DateTime.Now.ToLongTimeString() + ": Wrote " + fileName.Substring(tbLogFolder.Text.Length + 1);
        }

        public void UpdateButtonState()
        {
            if (!loggingRequest)
            {
                btnFolderSelect.Enabled = true;
                tbLogFolder.Enabled = true;
                cbGame.Enabled = true;
                btnLogCtl.Text = "Start logging";
                if (Directory.Exists(tbLogFolder.Text))
                {
                    btnLogCtl.Enabled = true;

                }
                else
                {
                    btnLogCtl.Enabled = false;
                }
            }
            else
            {
                cbGame.Enabled = false;
                btnFolderSelect.Enabled = false;
                tbLogFolder.Enabled = false;
                btnLogCtl.Enabled = true;
                btnLogCtl.Text = "Stop logging";
            }
        }

        public void UpdateGameProcess()
        {
            try
            {
                if (gameProcess == null)
                {
                    Process myProc = null;
                    try
                    {
                        myProc = Process.GetProcessesByName(gameData[cbGame.SelectedIndex].executableName).OrderByDescending(x => x.StartTime)
                        .FirstOrDefault(x => !x.HasExited);
                    }
                    catch (Exception e)
                    {
                        myProc = null;
                    }
                    if (myProc == null)
                        return;
                    gameProcess = myProc;
                }
                else if (gameProcess.HasExited)
                {
                    gameProcess = null;
                }
            }
            catch (Exception e)
            {
                gameProcess = null;
            }
            UpdateStateLbl();
        }

        public void UpdateStateLbl()
        {
            lblState.Text = (gameProcess != null ? "Connected" : "Disconnected") + " | " + (loggingRequest ? "Logging" : "Not Logging");
        }

        private void tbLogFolder_TextChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void cbGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            gameProcess = null;
        }

        private void RunLogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loggingInProg && dataBuf.Count()>0)
            {
                WriteOutBuffer();
            }
        }
    }
}
