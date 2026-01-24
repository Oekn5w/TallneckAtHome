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
using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;

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
            public string abbreviation;
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
        OBSWebsocket obs;
        string obs_url = "ws://127.0.0.1:4455";
        string obs_pw = "";
        int gameIdx = 0;

        public RunLogger()
        {
            InitializeComponent();
            gameData = new GameData[2];
            gameData[0].executableName = "HorizonZeroDawn";
            gameData[0].abbreviation = "HZD";
            gameData[0].gamePtrDef = new GameDataEntry[]
            {
                new GameDataEntry("posX",new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x120}),"d2",true),
                new GameDataEntry("posY",new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x128}),"d2",true),
                new GameDataEntry("posZ",new DeepPointer("", 0x0714cdc8, new int[] {0x30,0x130}),"d2",true),
                new GameDataEntry("igt",new DeepPointer("", 0x0714F830, new int[] {0x160}),"d3",true),
                new GameDataEntry("load",new DeepPointer("", 0x0714F830, new int[] {0x4B4}),"inz",true),
                new GameDataEntry("paused",new DeepPointer("", 0x0714F830, new int[] {0x20}),"inz",true),
                new GameDataEntry("invul",new DeepPointer("", 0x0714cdc8, new int[] {0x30, 0x1F0, 0x60}),"bnz",true),
                new GameDataEntry("mounted",new DeepPointer("", 0x0714cdc8, new int[] {0x30, 0x1C8}),"lnz",true)
            };
            gameData[1].executableName = "HorizonForbiddenWest";
            gameData[1].abbreviation = "HFW";
            gameData[1].gamePtrDef = new GameDataEntry[]
            {
                new GameDataEntry("posX",new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xD8}),"d2",true),
                new GameDataEntry("posY",new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xE0}),"d2",true),
                new GameDataEntry("posZ",new DeepPointer("", 0x8982DA0, new int[] { 0x1C10, 0x0, 0x10, 0xE8}),"d2",true),
                new GameDataEntry("igt",new DeepPointer("", 0x08983150, new int[] {0x120}),"d3",true),
                new GameDataEntry("load",new DeepPointer("", 0x08983150, new int[] {0x4B4}),"inz",true),
                new GameDataEntry("paused",new DeepPointer("", 0x08983150, new int[] {0x20}),"inz",true),
                new GameDataEntry("invul",new DeepPointer("", 0x8982DA0, new int[] {0x1C10, 0x0, 0x10, 0xD0, 0x70}),"bnz",true),
                new GameDataEntry("mountDestructability",new DeepPointer("", 0x08982DA0, new int[] {0x1C10, 0x0, 0x10, 0x80, 0xD0}),"ptr",true)
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
            lblOBS.Text = "OBS disconnected";
            obs = new OBSWebsocket();
            obs.Connected += onOBSConnect;
            obs.Disconnected += onOBSDisconnect;
            obs.RecordStateChanged += onOBSRecordStateChanged;
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 1; i < args.Length; ++i)
            {
                switch (args[i])
                {
                    case "--dir":
                        tbLogFolder.Text = args[++i];
                        break;
                    case "--obsurl":
                        obs_url = args[++i];
                        break;
                    case "--obspw":
                        obs_pw = args[++i];
                        break;
                    default:
                        break;
                }
            }
            if (obs_pw == "")
            {
                obs_pw = Environment.GetEnvironmentVariable("OBS_WS_PW");
                if(obs_pw ==null)
                {
                    obs_pw = "";
                }
            }
            timerOBS_Tick(null, null);
        }

        private void btnLogCtl_Click(object sender, EventArgs e)
        {
            if (!loggingRequest)
            {
                RequestStartLogging();
            }
            else
            {
                RequestStopLogging();
            }
        }

        private void RequestStartLogging()
        {
            if (!Directory.Exists(tbLogFolder.Text))
            {
                return;
            }
            loggingRequest = true;
            UpdateButtonState();
            UpdateStateLbl();
        }

        private void RequestStopLogging()
        {
            loggingRequest = false;
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
                int nData = gameData[gameIdx].gamePtrDef.Length;
                string temp;
                for (int i = 0; i < nData; i++)
                {
                    if (!gameData[gameIdx].gamePtrDef[i].toLog)
                        continue;
                    try
                    {
                        temp = getDataEntry(gameData[gameIdx].gamePtrDef[i]);
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
                uint value;
                if (entry.type.EndsWith("nz"))
                {
                    value = entry.ptr.Deref<uint>(gameProcess, 0);
                    return (value > 0).ToString();
                }
                else
                {
                    value = entry.ptr.Deref<uint>(gameProcess, uint.MaxValue);
                    if (entry.type.EndsWith("x"))
                        return "0x" + value.ToString("X08");
                    else
                        return value.ToString();
                }
            }
            else if (entry.type.StartsWith("l"))
            {
                ulong value;
                if (entry.type.EndsWith("nz"))
                {
                    value = entry.ptr.Deref<ulong>(gameProcess, 0);
                    return (value > 0).ToString();
                }
                else
                {
                    value = entry.ptr.Deref<ulong>(gameProcess, ulong.MaxValue);
                    if (entry.type.EndsWith("x"))
                        return "0x" + value.ToString("X016");
                    else
                        return value.ToString();
                }
            }
            else if (entry.type == "ptr")
            {
                ulong value;
                value = entry.ptr.Deref<ulong>(gameProcess, ulong.MaxValue);
                int retVal;
                if (value == 0)
                {
                    retVal = 0;
                }
                else if (value == ulong.MaxValue)
                {
                    retVal = -1;
                }
                else
                {
                    retVal = 1;
                }
                return retVal.ToString();
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
            lblBuffer.Text = "";
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
                        if (!loggingRequest || dataBuf.Count() == 0)
                        {
                            if (cbGame.SelectedIndex != 0)
                            {
                                int i;
                                for (i = 0; i < gameData.Length; ++i)
                                {
                                    if (gameData[i].abbreviation == (string)cbGame.SelectedItem)
                                    {
                                        gameIdx = i;
                                        break;
                                    }
                                }
                                if (i == gameData.Length)
                                { // no match, shouldn't happen
                                    return;
                                }
                            }
                            else
                            {
                                gameIdx++;
                                gameIdx = gameIdx % gameData.Length;
                            }
                        }
                        myProc = Process.GetProcessesByName(gameData[gameIdx].executableName).OrderByDescending(x => x.StartTime)
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
            string temp = "";
            if (gameProcess != null)
            {
                if (cbGame.SelectedIndex == 0)
                {
                    temp = gameData[gameIdx].abbreviation + " ";
                }
                temp += "Connected";
            }
            else
            {
                temp = "Disconnected";
            }
            lblState.Text = temp + " | " + (loggingRequest ? "Logging" : "Not Logging");
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
            if (loggingInProg && dataBuf.Count() > 0)
            {
                WriteOutBuffer();
            }
        }


        private void onOBSConnect(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                lblOBS.Text = "OBS connected";
                btnSyncOBS.Enabled = true;
                SyncOBSStatus();
            }));

        }

        private void SyncOBSStatus()
        {
            if (!cbOBS.Checked || !obs.IsConnected)
            {
                return;
            }
            var curRecState = obs.GetRecordStatus();
            if (curRecState.IsRecording)
            {
                if (Directory.Exists(tbLogFolder.Text) && !loggingRequest)
                {
                    RequestStartLogging();
                }
            }
            else
            {
                if (loggingRequest)
                {
                    RequestStopLogging();
                }
            }
        }

        private void onOBSDisconnect(object sender, OBSWebsocketDotNet.Communication.ObsDisconnectionInfo e)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                lblOBS.Text = "OBS disconnected";
                btnSyncOBS.Enabled = false;
            }));
        }

        private void onOBSRecordStateChanged(object sender, RecordStateChangedEventArgs args)
        {
            if (!cbOBS.Checked)
            {
                return;
            }
            BeginInvoke((MethodInvoker)(() =>
            {

                switch (args.OutputState.State)
                {
                    case OutputState.OBS_WEBSOCKET_OUTPUT_STARTED:
                    case OutputState.OBS_WEBSOCKET_OUTPUT_RESUMED:
                        RequestStartLogging();
                        break;

                    case OutputState.OBS_WEBSOCKET_OUTPUT_STOPPED:
                    case OutputState.OBS_WEBSOCKET_OUTPUT_PAUSED:
                        RequestStopLogging();
                        break;

                    default:
                        break;
                }
            }));
        }

        private void timerOBS_Tick(object sender, EventArgs e)
        {
            if (!obs.IsConnected)
            {
                obs.ConnectAsync(obs_url, obs_pw);
            }
        }

        private void btnSyncOBS_Click(object sender, EventArgs e)
        {
            SyncOBSStatus();
        }
    }
}
