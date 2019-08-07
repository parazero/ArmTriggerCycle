using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Net.Mail;
using System.Xml;
using System.IO;

namespace TestsDisplays
{
    public partial class Form1 : Form
    {
        static int localPort;
        static int localBasePort;
        static int remotePort;
        static int remoteBasePort;
        static UdpClient tesyUDP;
        static string ArduinoComParam = "";
        static string SMAComParam = "";
        static string strCmdText = "";
        static string TestNameParam = "";
        string PCName = "";
        static Process p;

        public int PIDValue = 0;

        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        UdpClient udpClient = new UdpClient();
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        bool _continue;
        public bool IsMailSentToday = true;

        public delegate void AddDataDelegate(String instring, TextBox textBoxToUpdate);
        public AddDataDelegate textBoxUpdateDelegate;

        public System.Timers.Timer WatchdogTimer = new System.Timers.Timer(1000 * 60);

        public Form1()
        {

            localBasePort = 11000; // Opposite than AppConsole
            remoteBasePort = 13000;

            InitializeComponent();

            textBoxUpdateDelegate = new AddDataDelegate(AddTextTotextBoxMethod);

            TestTypecomboBox.SelectedIndexChanged += new EventHandler(TestTypeChange_Method);

            foreach (string s in SerialPort.GetPortNames())
            {
                ArduinoPortcomboBox.Items.Add(s);
                SmartAirPortcomboBox.Items.Add(s);

            }
            PCName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
            WatchdogTimer.Elapsed += OnTimedEvent;
            WatchdogTimer.AutoReset = true;
            WatchdogTimer.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async Task WriteDataAsync(UdpClient udpClient)
        {
            while (_continue)
            {
                var receivedResults = await udpClient.ReceiveAsync();
                string returnData = Encoding.ASCII.GetString(receivedResults.Buffer);

                int valueIndex = returnData.IndexOf("Value:");
                int errorIndex = returnData.IndexOf("Error:");
                int value2Index = returnData.IndexOf("Value2:");
                int error2Index = returnData.IndexOf("Error2:");
                int PIDIndex = returnData.IndexOf("PID:");
                int eolIndex = returnData.IndexOf("EOL");

                int valueLength = errorIndex - valueIndex;
                int errorLength = value2Index - errorIndex;
                int value2Length = error2Index - value2Index;
                int error2Length = PIDIndex - error2Index;
                int PIDLength = eolIndex - PIDIndex;
                PIDValue = Convert.ToInt32(returnData.Substring(PIDIndex + 4, PIDLength - 4));
                //RemoteChannellabel.Text = "Remote Channel ID: " + PIDValue.ToString();
                if (returnData.Contains("MotorSignal:"))
                {
                    //MotorSignal: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("MotorSignal:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }

                if (returnData.Contains("Modes:"))
                {
                    //Modes: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("Modes:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }

                if (returnData.Contains("ArmDisarm:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("ArmDisarm:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }
                if (returnData.Contains("Discharge:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("Discharge:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }

                if (returnData.Contains("XBTTest:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("XBTTest:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }

                if (returnData.Contains("XBTTestPowerUp:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("XBTTestPowerUp:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }
                if (returnData.Contains("ArmAtStartUp:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("ArmAtStartUp:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                }
                //
                if (returnData.Contains("TestAutoPort:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("TestAutoPort:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                    if (returnData.Equals("exit"))
                        _continue = false;
                }

                if (returnData.Contains("PWMToRelay:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("PWMToRelay:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                    if (returnData.Equals("exit"))
                        _continue = false;
                }
                if (returnData.Contains("PWMToRelaySoftReset:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("PWMToRelaySoftReset:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                    if (returnData.Equals("exit"))
                        _continue = false;
                }
                if (returnData.Contains("PWMVoltageAtPowerCycle:"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("PWMVoltageAtPowerCycle:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                    if (returnData.Equals("exit"))
                        _continue = false;
                }
                if (returnData.Contains("Q10Voltage"))
                {
                    //ArmDisarm: Value: 9999999 Error: 9999988 Value2: 9999977 Error2: 9999966 EOL
                    int testIndex = returnData.IndexOf("Q10Voltage:");
                    int testLength = valueIndex - testIndex;
                    int valueValue = Convert.ToInt32(returnData.Substring(valueIndex + 7, valueLength - 8));
                    int errorValue = Convert.ToInt32(returnData.Substring(errorIndex + 7, errorLength - 8));
                    int value2Value = Convert.ToInt32(returnData.Substring(value2Index + 7, value2Length - 8));
                    int error2Value = Convert.ToInt32(returnData.Substring(error2Index + 7, error2Length - 8));
                    PassedTest1textBox.Invoke(textBoxUpdateDelegate, new Object[] { valueValue.ToString(), PassedTest1textBox });
                    FailedTests1textBox.Invoke(textBoxUpdateDelegate, new Object[] { errorValue.ToString(), FailedTests1textBox });
                    passedTests3textBox.Invoke(textBoxUpdateDelegate, new Object[] { value2Value.ToString(), passedTests3textBox });
                    failedTests2textBox.Invoke(textBoxUpdateDelegate, new Object[] { error2Value.ToString(), failedTests2textBox });
                    if (returnData.Equals("exit"))
                        _continue = false;
                }
            }

        }

        public void AddTextTotextBoxMethod(String myString, TextBox textBoxToUpdate)
        {
            textBoxToUpdate.Text = myString;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            int typeIndex = TestTypecomboBox.SelectedIndex;
            //string TestNameForStartup = "";
            //UdpClient udpClient = new UdpClient(localBasePort + typeIndex);
            Process currentProcess = Process.GetCurrentProcess();
            //Process[] pname = Process.GetProcessesByName("TestsDisplays");
            if (buttonStart.Text.Equals("Start"))
            {
                _continue = true;
                buttonStart.Text = "Stop";
                ChannelIDLabel.Text = "Channel ID: " + currentProcess.Id.ToString();//Remote Channel ID:
                //udpClient = new UdpClient(localBasePort + typeIndex);
                //udpClient.Connect(localAddr, remoteBasePort + typeIndex);
                udpClient = new UdpClient(localBasePort + currentProcess.Id);
                udpClient.Connect(localAddr, remoteBasePort + currentProcess.Id);
                if (typeIndex.Equals(0))// PWM and Cutoff signal
                {
                    label1.Text = "PWM Passed tests:";
                    label2.Text = "PWM Failed tests:";
                    PassedTests2label.Text = "Cutoff signal passed tests:";
                    failedTests2label.Text = "Cutoff signal failed tests:";
                    PassedTests2label.Visible = true;
                    failedTests2label.Visible = true;
                    passedTests3textBox.Visible = true;
                    failedTests2textBox.Visible = true;

                    TestName.Text = "Motor cutoff signal and PWM length test";
                    TestName.Visible = true;
                    TestNameParam = "MotorSignalDetection";

                }
                if (typeIndex.Equals(1)) // Modes
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "Auto / Manual / Maintenance modes cycles";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "Modes";
                }
                if (typeIndex.Equals(2))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "Arm / Disarm cycles";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "ArmDisarm";
                }
                if (typeIndex.Equals(3))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "Discharge cycles";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "Discharge";
                }
                if (typeIndex.Equals(4))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "XBT Disarm cycles";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "XBTTest";
                }
                if (typeIndex.Equals(5))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "XBT Init failure cycles";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "XBTTestPowerUp";
                }
                if (typeIndex.Equals(6))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "Arm At Init tests";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "ArmAtStartUp";
                }
                if (typeIndex.Equals(7))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "Trigger Due to RC";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "ArmAtStartUp";
                }
                if (typeIndex.Equals(8))
                {
                    label1.Text = "Passed tests:";
                    label2.Text = "Failed tests:";
                    TestName.Text = "Trigger Due to RC";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "TestAutoPort";
                }
                if (typeIndex.Equals(9))
                {
                    label1.Text = "Idle & Arm Passed tests:";
                    label2.Text = "Idle & Arm Failed tests:";
                    TestName.Text = "PWM Signal to Relay";
                    TestName.Visible = true;
                    PassedTests2label.Visible = true;
                    failedTests2label.Visible = true;
                    PassedTests2label.Text = "Trigger Passed tests:";
                    failedTests2label.Text = "Trigger Failed tests:";
                    passedTests3textBox.Visible = true;
                    failedTests2textBox.Visible = true;
                    TestNameParam = "PWMToRelay";
                }
                if (typeIndex.Equals(10))
                {
                    label1.Text = "Soft reset Passed tests:";
                    label2.Text = "Soft reset Failed tests:";
                    TestName.Text = "PWM @Soft reset";
                    TestName.Visible = true;
                    PassedTests2label.Visible = false;
                    failedTests2label.Visible = false;
                    PassedTests2label.Text = "Trigger Passed tests:";
                    failedTests2label.Text = "Trigger Failed tests:";
                    passedTests3textBox.Visible = false;
                    failedTests2textBox.Visible = false;
                    TestNameParam = "PWMToRelaySoftReset";
                }
                if (typeIndex.Equals(11))
                {
                    label1.Text = "Hard Pwr cycle Passed:";
                    label2.Text = "Hard Pwr cycle Failed:";
                    TestName.Text = "PWM Signal & Voltage at Hard power cycle";
                    TestName.Visible = true;
                    PassedTests2label.Visible = true;
                    failedTests2label.Visible = true;
                    PassedTests2label.Text = "Relay Voltage Passed tests:";
                    failedTests2label.Text = "Relay Voltage Failed tests:";
                    passedTests3textBox.Visible = true;
                    failedTests2textBox.Visible = true;
                    TestNameParam = "PWMVoltageAtPowerCycle";
                }
                if (typeIndex.Equals(12))
                {
                    label1.Text = "Q10 Init cycle Passed:";
                    label2.Text = "Q10 Init cycle Failed:";
                    TestName.Text = "Q10 Voltage at Soft power cycle";
                    TestName.Visible = true;
                    PassedTests2label.Visible = true;
                    failedTests2label.Visible = true;
                    PassedTests2label.Text = "Q10 trigger cycle Passed:";
                    failedTests2label.Text = "Q10 trigger cycle Failed:";
                    passedTests3textBox.Visible = true;
                    failedTests2textBox.Visible = true;
                    TestNameParam = "Q10Voltage";
                }
                Thread writeThread = new Thread(() => WriteDataAsync(udpClient));
                writeThread.Start();
                strCmdText = "/C ConsoleSerialPortReader.exe " + TestNameParam + " " + SMAComParam + " " + ArduinoComParam + " " + currentProcess.Id.ToString();
                Process p = Process.Start("CMD.exe", strCmdText);

            }
            else if (buttonStart.Text.Equals("Stop"))
            {
                Process[] W = Process.GetProcesses();
                foreach (Process w in W)
                {
                    if (w.Id.Equals(PIDValue) && !PIDValue.Equals(0))
                    {
                        w.Kill();
                    }
                    //TestTypecomboBox.Items.Add(w.ProcessName);
                }
                _continue = false;
                buttonStart.Text = "Start";
                udpClient.Close();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ArduinoPortbutton_Click(object sender, EventArgs e)
        {
            ArduinoComParam = ArduinoPortcomboBox.Text;
        }

        private void SMAPortbutton_Click(object sender, EventArgs e)
        {
            SMAComParam = SmartAirPortcomboBox.Text;
        }

        private void TestTypeChange_Method(object sender, EventArgs e)
        {
            if (TestTypecomboBox.SelectedIndex.Equals(9))
            {
                MessageBox.Show("Make sure:\r\n" +
                                "PSTO is set to 20 seconds,\r\n" +
                                "MSTO is set to 50 seconds,\r\n" +
                                "PSOF is set to 1000,\r\n" +
                                "PSON is set to 1900,\r\n" +
                                "MSOF is set to 1000,\r\n" +
                                "MSON is set to 1900,\r\n" +
                                "PWM is set to 1,\r\n" +
                                "MPWM is set to 1,\r\n" +
                                "NVI is set to 400,\r\n" +
                                "Arduino_Pyro_Nano_ReadPWMWithCommands is loaded to arduino.", "Message");
            }

            if (TestTypecomboBox.SelectedIndex.Equals(10))
            {
                MessageBox.Show("Make sure:\r\n" +
                                "PSOF is set to 1000,\r\n" +
                                "PSON is set to 1900,\r\n" +
                                "PWM is set to 1,\r\n" +
                                "Arduino_Pyro_Nano_ReadPWMWithCommands is loaded to arduino.", "Message");
            }
            if (TestTypecomboBox.SelectedIndex.Equals(11))
            {
                MessageBox.Show("Make sure:\r\n" +
                                "PSTO is set to 20 seconds,\r\n" +
                                "PSOF is set to 1000,\r\n" +
                                "PSON is set to 1900,\r\n" +
                                "PWM is set to 1,\r\n" +
                                "Arduino_Pyro_Nano_ReadPWMWithCommands is loaded to arduino.", "Message");
            }
            if (TestTypecomboBox.SelectedIndex.Equals(12))
            {
                MessageBox.Show("Make sure:\r\n" +
                                "SVPY is set to 2,\r\n" +
                                "NVI is set to 400,\r\n" +
                                "Arduino_Pyro_Nano_ReadPWMWithCommands is loaded to arduino.", "Message");
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (buttonStart.Text.Equals("Stop"))
            {

                try
                {
                    if (DateTime.Now.Hour.Equals(8) && !IsMailSentToday)
                    {
                        IsMailSentToday = true;
                        string MailBody = "PC Name: " + PCName + "\r\n" +
                        "Test: " + TestName.Text + "\r\n" +
                        "PassedTest #1: " + PassedTest1textBox.Text + "\r\n" +
                        "FailedTest #1: " + FailedTests1textBox.Text + "\r\n" +
                        "PassedTest #2: " + passedTests3textBox.Text + "\r\n" +
                        "FailedTest #2: " + failedTests2textBox.Text + "\r\n";
                        XmlDocument doc = new XmlDocument();
                        doc.Load(".\\Resources\\emailList.xml");
                        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                        {
                            string text = node.InnerText; //or loop through its children as well
                            SendMail(text, "Long run test - Daily report - " + TestName.Text, MailBody);
                        }
                        
                    }
                    else if (!DateTime.Now.Hour.Equals(8) && IsMailSentToday)
                    {
                        IsMailSentToday = false;
                    }
                    Process W = Process.GetProcessById(PIDValue);// GetProcesses();
                }
                catch (Exception Ex)//if (W.HasExited)
                {
                    string MailBody = "PC Name: " + PCName + "\r\n" +
                        "Test: " + TestName.Text + "\r\n" +
                        "PassedTest #1: " + PassedTest1textBox.Text + "\r\n" +
                        "FailedTest #1: " + FailedTests1textBox.Text + "\r\n" +
                        "PassedTest #2: " + passedTests3textBox.Text + "\r\n" +
                        "FailedTest #2: " + failedTests2textBox.Text + "\r\n";
                    XmlDocument doc = new XmlDocument(); 
                    doc.Load(".\\Resources\\emailList.xml");
                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        string text = node.InnerText; //or loop through its children as well
                        SendMail(text, "Long run test - Crash report - " + TestName.Text, MailBody);
                    }
                    Process p = Process.Start("CMD.exe", strCmdText);
                }
            }
        }

        private void SendMail(string SendTo, string MailSubject, string MailBody)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("parazeroauto@gmail.com");
            mail.To.Add(SendTo);
            mail.Subject = MailSubject;
            mail.Body = MailBody;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("parazeroauto", "fdfdfd3030");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}
