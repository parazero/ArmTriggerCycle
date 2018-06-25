using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestsDisplays
{
    public partial class Form1 : Form
    {
        static int localPort;
        static int localBasePort;
        static int remotePort;
        static int remoteBasePort;
        static UdpClient tesyUDP;

        IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        UdpClient udpClient = new UdpClient();
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        bool _continue;

        public delegate void AddDataDelegate(String instring, TextBox textBoxToUpdate);
        public AddDataDelegate textBoxUpdateDelegate;

        public Form1()
        {

            localBasePort = 11000; // Opposite than AppConsole
            remoteBasePort = 13000;
            //buttonStart.Click += new EventHandler(buttonStart_Click);


            InitializeComponent();
            //udpClient.Connect(localAddr, 13000);

            textBoxUpdateDelegate = new AddDataDelegate(AddTextTotextBoxMethod);

            //Thread writeThread = new Thread(WriteData);
            //writeThread.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async Task WriteDataAsync(UdpClient udpClient)
        {
            while (_continue)
            {
                /*Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);*/
                var receivedResults = await udpClient.ReceiveAsync();
                string returnData = Encoding.ASCII.GetString(receivedResults.Buffer);

                int valueIndex = returnData.IndexOf("Value:");
                int errorIndex = returnData.IndexOf("Error:");
                int value2Index = returnData.IndexOf("Value2:");
                int error2Index = returnData.IndexOf("Error2:");
                int eolIndex = returnData.IndexOf("EOL");

                int valueLength = errorIndex - valueIndex;
                int errorLength = value2Index - errorIndex;
                int value2Length = error2Index - value2Index;
                int error2Length = eolIndex - error2Index;

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
                if (returnData.Equals("exit"))
                    _continue = false;
                
            }
            
        }

        public void AddTextTotextBoxMethod(String myString, TextBox textBoxToUpdate)
        {
            textBoxToUpdate.Text = myString;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            int typeIndex = TestTypecomboBox.SelectedIndex;
            //UdpClient udpClient = new UdpClient(localBasePort + typeIndex);
            
            if (buttonStart.Text.Equals("Start"))
            {
                _continue = true;
                buttonStart.Text = "Stop";
                udpClient = new UdpClient(localBasePort + typeIndex);
                udpClient.Connect(localAddr, remoteBasePort + typeIndex);
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
                }
                Thread writeThread = new Thread(() => WriteDataAsync(udpClient));
                writeThread.Start();
            }
            else if (buttonStart.Text.Equals("Stop"))
            {
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
    }
}
