// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using log4net.Appender;
using log4net.Config;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ConsoleSerialPortReader;
using System.Net.Mail;

public class PortChat
{
    static bool _continue;
    static SerialPort _serialPort;
    static SerialPort ArduinoPort = new SerialPort();
    static string param;
    static bool detected_Motor_signal_on;
    static long trigger_with_Motor_High_Counter;
    static long trigger_with_Motor_High_error_Counter;
    static long PWM_width_Counter;
    static long PWM_width_error_Counter;
    static bool PreviousModeIsMaintenance;
    static bool PreviousModeIsManual;
    static bool PreviousModeIsAuto;
    static long Mode_Transitions_Counter;
    static long Mode_Transitions_Error_Counter;
    static long Mode_Cycles_Indeicator;
    static bool PreviousStateIsIdle;
    static long State_Cycles_Indeicator;
    static bool PreviousStateIsArmed;
    static bool PreviousStateIsDisarmed;
    static long State_Transitions_Counter;
    static long State_Transitions_Error_Counter;
    static int localPort;
    static int localBasePort;
    static int remotePort;
    static int remoteBasePort;
    static int portOffset;
    static long discharge_Counter;
    static bool dischargeInitCycle;
    static long XBT_disarm_Counter;
    static long XBT_disarm_Counter_Error;
    static long Successful_Init_Counter;
    static long Successful_Init_Counter_Error;
    static bool WaitForInit = false;
    static private bool TestTimeout = false;
    static bool EndCondition = false;
    static bool IdentifiedText = false;
    static bool WaitForReset = false;

    static long General_Counter = 0;
    static long General_Counter_Error = 0;

    static int SleepDurationAfterBoardInitMethod = 3000;
    static int SleepDurationAfterResetEvent = 15000;
    static int SleepDuaraionAfterPortInit = 1000;
    static int SleepDurationForFullInit = 25000;
    static int SleepAfterWriteLineEvent = 3000;
    static int SleepAfterDisarmEvent = 4500;
    static int SleepForArmDuration = 2000;
    static int SleepAfterArduinoFlash = 4500;
    static int LongTest = 0;
    static int TestTimeOutDuration = 35000;
    static public int PID;

    static string message;
    static string CurrDir = "";
    static string LogName1 = "";
    static string FullTextSmartAir = "";

    static public string FullTextArduino = "";

    static string ArduinoCOMPort = "COM3";

    static public Stopwatch stopWatch = new Stopwatch();

    static Stopwatch resetStopWatch;


    static IPAddress localAddr = IPAddress.Parse("127.0.0.1");

    static TimeSpan ts;


    //private static readonly log4net.ILog log =
    //        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    //private static readonly CWDFileAppender wwww = new CWDFileAppender();

    private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(typeof(PortChat));


    public static void Main(string[] args)
    {
        CurrDir = Directory.GetCurrentDirectory();

        //UpdatelogFournetfile();
        trigger_with_Motor_High_Counter = 0;
        trigger_with_Motor_High_error_Counter = 0;
        PWM_width_Counter = 0;
        PWM_width_error_Counter = 0;
        discharge_Counter = 0;
        detected_Motor_signal_on = false;
        PreviousModeIsMaintenance = false;
        PreviousModeIsAuto = false;
        PreviousModeIsManual = false;
        dischargeInitCycle = true;
        Mode_Transitions_Counter = 0;
        Mode_Transitions_Error_Counter = 0;
        Mode_Cycles_Indeicator = 0;
        localBasePort = 13000;
        remoteBasePort = 11000;
        portOffset = 0;
        XBT_disarm_Counter = 0;
        XBT_disarm_Counter_Error = 0;
        Successful_Init_Counter = 0;
        Successful_Init_Counter_Error = 0;

        Process currentProcess = Process.GetCurrentProcess();
        PID = currentProcess.Id;

        param = args[0];
        if (args.Length.Equals(4))
        {
            portOffset = Convert.ToInt32(args[3]);
        }

        localPort = localBasePort + portOffset;
        remotePort = remoteBasePort + portOffset;
        /*if (param.Equals("MotorSignalDetection"))
        {
            localPort = localBasePort;
            remotePort = remoteBasePort;
        }
        else if (param.Equals("Modes"))
        {
            localPort = localBasePort + 1;
            remotePort = remoteBasePort + 1;
        }
        else if (param.Equals("ArmDisarm"))
        {
            localPort = localBasePort + 2;
            remotePort = remoteBasePort + 2;
        }
        else if (param.Equals("Discharge"))
        {
            localPort = localBasePort + 3;
            remotePort = remoteBasePort + 3;
        }

        else if (param.Equals("XBTTest"))
        {
            localPort = localBasePort + 4;
            remotePort = remoteBasePort + 4;
        }

        else if (param.Equals("XBTTestPowerUp"))
        {
            localPort = localBasePort + 5;
            remotePort = remoteBasePort + 5;
        }

        else if (param.Equals("ArmAtStartUp"))
        {
            localPort = localBasePort + 6;
            remotePort = remoteBasePort + 6;
        }

        else if (param.Equals("TriggerDueToRC"))
        {
            localPort = localBasePort + 7;
            remotePort = remoteBasePort + 7;
        }

        else if (param.Equals("DisarmDueToNoVib"))
        {
            localPort = localBasePort + 8;
            remotePort = remoteBasePort + 8;
        }*/
        //LogName1 = "d.log";//param + "_" + DateTime.Now.ToString().Replace("/","-").Replace(" ","_").Replace(":","-") + ".log";
        //log4net.GlobalContext.Properties["LogName"] = LogName1;
        log.Debug(param);

        UdpClient udpClient = new UdpClient(localPort);
        string message;

        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(() => Read(udpClient));
        FileInfo qqq = new System.IO.FileInfo(Directory.GetCurrentDirectory());
        log.Debug(qqq.ToString());
        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort();
        // Allow the user to set the appropriate properties.
        if (args.Length.Equals(1))
        {
            _serialPort.PortName = SetPortName("COM6");
            _serialPort.BaudRate = SetPortBaudRate(921600);
            _serialPort.Parity = SetPortParity(_serialPort.Parity);
            _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
            log.Debug("Port Data: COM ID: " + _serialPort.PortName);

            ArduinoCOMPort = SetPortName(ArduinoCOMPort);
        }
        else if (args.Length.Equals(3))
        {
            _serialPort.PortName = args[1];
            _serialPort.BaudRate = 921600;
            _serialPort.Parity = 0;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = (StopBits)1;
            _serialPort.Handshake = 0;
            log.Debug("Port Data: COM ID: " + _serialPort.PortName);

            ArduinoCOMPort = args[2];
        }

        else if (args.Length.Equals(4))
        {
            _serialPort.PortName = args[1];
            _serialPort.BaudRate = 921600;
            _serialPort.Parity = 0;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = (StopBits)1;
            _serialPort.Handshake = 0;

            log.Debug("Port Data: COM ID: " + _serialPort.PortName);

            ArduinoCOMPort = args[2];
        }

        ArduinoPortInitialization();
        // Set the read/write timeouts
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;
        _serialPort.ReceivedBytesThreshold = 1;
        _serialPort.ReadBufferSize = 6000000;
        udpClient.Connect(localAddr, remotePort);
        _continue = true;
        if (!param.Equals("PWMVoltageAtPowerCycle"))
        {
            _serialPort.Open();

            readThread.Start();
        }


        if (param.Equals("PWMVoltageAtPowerCycle"))
        {
            Console.WriteLine("Send PWRUP to start test.");
        }
        Console.WriteLine("Type QUIT to exit");
        Console.WriteLine("Channel ID: " + portOffset.ToString());
        stopWatch.Start();
        SendToUI(udpClient, "InitData", 0, 0, 0, 0);
        while (_continue)
        {
            message = Console.ReadLine();
            log.Debug(message);
            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            else
            {
                if (!param.Equals("PWMVoltageAtPowerCycle"))
                {
                    _serialPort.WriteLine(String.Format("{0}\r\n", message));
                }
                if (stringComparer.Equals("MS", message))
                    SendToUI(udpClient, "MotorSignal", -1, 0, 0, 0);
                if (stringComparer.Equals("Modes", message))
                    SendToUI(udpClient, "Modes", 11, 22, 333, 412430);
                if (stringComparer.Equals("AD", message))
                    SendToUI(udpClient, "ArmDisarm", 99, 22222, 333, 412430);
                if (stringComparer.Equals("DIS", message))
                    SendToUI(udpClient, "Discharge", 34, 1432, 123, 87890);
                if (stringComparer.Equals("Start", message))
                    ArduinoPort.WriteLine("Rec");
                if (stringComparer.Equals("PWRUP", message))
                {
                    //readThread.Suspend();
                    //
                    WriteToArduino("PWRUP");
                    WriteToArduino("PWMREADServo");
                    ColoerdTimer(5000);
                    //_serialPort.Open();
                    //readThread.Resume();
                    try
                    {
                        _serialPort.Open();
                    }
                    catch (Exception ex)
                    {
                        log.Error("SmartAir Port failed.");
                        log.Error(ex);
                        if (ex.Message.Contains("Access to the port is denied"))
                        {
                            ResetSmartAir();
                        }
                        else
                        {
                            WriteToArduino("PWMREADServoStop");
                            WriteToArduino("PWRDWN");
                            ColoerdTimer(3000);
                            WriteToArduino("PWRUP");
                        }
                    }
                    if (!readThread.IsAlive)
                        readThread.Start();

                }
                if (stringComparer.Equals("PWRDWN", message))
                {
                    WriteToArduino("PWRDWN");
                    WriteToArduino("PWMREADServo");
                }
                if (stringComparer.Equals("PWMREAD", message))
                {
                    WriteToArduino("PWMREADServo");
                }
            }
        }

        readThread.Join();
        _serialPort.Close();
    }

    private static void ResetSmartAir()
    {
        WriteToArduino("PWMREADStop");
        WriteToArduino("PWRDWN");
        _serialPort.Close();
        ColoerdTimer(3000);
        _serialPort.Open();
        WriteToArduino("PWRUP");
    }

    public static void Read(UdpClient udpClient)
    {
        while (_continue)
        {
            try
            {
                if (_serialPort.IsOpen)
                {
                    message = _serialPort.ReadLine();
                    FullTextSmartAir += message;
                }
            }
            catch (TimeoutException)
            {
                if (_serialPort.IsOpen)
                {
                    message = _serialPort.ReadExisting();
                    FullTextSmartAir += message;
                }
            }
            if (!message.Length.Equals(0))
            {
                log.Debug(message);
                Console.WriteLine(message);
            }
            if (param.Equals("Motor"))
            {
                if (message.Contains("MOTOR_OFF"))
                {
                    Console.WriteLine("Reset in 1 Second");
                    log.Debug("Reset SmartAir in 1 second due to Motor off detection");
                    Thread.Sleep(5000);
                    _serialPort.Write("RST\r\n");
                }
            }
            if (param.Equals("MotorSignalDetection"))
            {

                //SendToUI(udpClient,"MotorSignal", PWM_width_Counter, PWM_width_error_Counter, trigger_with_Motor_High_Counter, trigger_with_Motor_High_error_Counter);
                bool preResetEvent = true;
                if (message.Contains("Motor Signal High") && !detected_Motor_signal_on)
                {
                    Console.WriteLine("Detected Motor Signal High");
                    log.Debug("Detected Motor Signal High");
                    detected_Motor_signal_on = true;
                }
                if (message.Contains("<0,0,"))
                    preResetEvent = false;
                if (message.Contains("<1,1,") && detected_Motor_signal_on)
                {
                    trigger_with_Motor_High_Counter++;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Arm - Trigger Cycle Counter: " + trigger_with_Motor_High_Counter.ToString());
                    Console.ResetColor();
                    log.Debug("SmartAir Indicated triggering after Motor High signal - Count: " + trigger_with_Motor_High_Counter.ToString());
                    detected_Motor_signal_on = false;
                    SendToUI(udpClient, "MotorSignal", PWM_width_Counter, PWM_width_error_Counter, trigger_with_Motor_High_Counter, trigger_with_Motor_High_error_Counter);
                }
                else if (message.Contains("<") && !message.Contains("<1,1,") && detected_Motor_signal_on && !preResetEvent)
                {
                    trigger_with_Motor_High_error_Counter++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error Cycle Counter: " + trigger_with_Motor_High_error_Counter.ToString());
                    Console.ResetColor();
                    log.Error("Error cycle (motor signal without Trigger) - Count: " + trigger_with_Motor_High_error_Counter.ToString());
                    detected_Motor_signal_on = false;
                    SendToUI(udpClient, "MotorSignal", PWM_width_Counter, PWM_width_error_Counter, trigger_with_Motor_High_Counter, trigger_with_Motor_High_error_Counter);
                }
                if (message.Contains("PWM Pulse Width"))
                {
                    if (Convert.ToInt32(message.Substring(17, message.Length - 17)) >= 900 && Convert.ToInt32(message.Substring(17, message.Length - 17)) <= 1100)
                    {
                        PWM_width_Counter++;
                    }
                    else
                    {
                        PWM_width_error_Counter++;
                    }

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("PWM Width Counter: " + PWM_width_Counter.ToString());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("PWM Width Error Counter:" + PWM_width_error_Counter.ToString());
                    Console.ResetColor();
                    log.Debug("PWM Width Counter: " + PWM_width_Counter.ToString() + " PWM Width Error Counter:" + PWM_width_error_Counter.ToString());
                    SendToUI(udpClient, "MotorSignal", PWM_width_Counter, PWM_width_error_Counter, trigger_with_Motor_High_Counter, trigger_with_Motor_High_error_Counter);
                }
            }
            if (param.Equals("Modes"))
            {
                bool FirstCycle = !(PreviousModeIsAuto || PreviousModeIsMaintenance || PreviousModeIsManual);
                if (message.Contains("Maintenance mode") && ((PreviousModeIsManual == true) || FirstCycle))
                {
                    Console.WriteLine("Maintenance Mode");
                    log.Debug("Maintenance Mode");
                    PreviousModeIsMaintenance = true;
                    PreviousModeIsAuto = false;
                    PreviousModeIsManual = false;
                    Mode_Cycles_Indeicator = Mode_Cycles_Indeicator + 1;
                }
                else if (message.Contains("Maintenance mode") && !((PreviousModeIsManual == true) || FirstCycle))
                {
                    if (!PreviousModeIsMaintenance)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Maintenance Mode not after manual mode: " + Mode_Transitions_Counter.ToString());
                        Console.WriteLine("Mnt: {0}, Auto: {1}, Man: {2}", PreviousModeIsMaintenance, PreviousModeIsAuto, PreviousModeIsManual);
                        Console.ResetColor();
                        log.Error("Maintenance Mode not after manual mode: " + Mode_Transitions_Counter.ToString());
                        Mode_Cycles_Indeicator = -1;
                        Mode_Transitions_Error_Counter++;
                    }
                    else
                    {
                        Console.WriteLine("Duplicate maintenance.");
                        log.Error("Duplicate maintenance line entry.");
                    }
                }

                if (message.Contains("Auto triggering") && ((PreviousModeIsMaintenance == true) || FirstCycle))
                {
                    Console.WriteLine("Auto triggering Mode");
                    log.Debug("Auto triggering Mode");
                    PreviousModeIsAuto = true;
                    PreviousModeIsManual = false;
                    PreviousModeIsMaintenance = false;
                    Mode_Cycles_Indeicator = Mode_Cycles_Indeicator + 2;
                }
                else if (message.Contains("Auto triggering") && !((PreviousModeIsMaintenance == true) || FirstCycle))
                {
                    if (!PreviousModeIsAuto)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Auto Mode not after maintenance mode: " + Mode_Transitions_Counter.ToString());
                        Console.WriteLine("Mnt: {0}, Auto: {1}, Man: {2}", PreviousModeIsMaintenance, PreviousModeIsAuto, PreviousModeIsManual);
                        Console.ResetColor();
                        log.Error("Auto Mode not after maintenance mode: " + Mode_Transitions_Counter.ToString());
                        Mode_Cycles_Indeicator = -1;
                        Mode_Transitions_Error_Counter++;
                    }
                    else
                    {
                        Console.WriteLine("Duplicate auto.");
                        log.Error("Duplicate auto line entry.");
                    }
                }

                if (message.Contains("Manual triggering") && ((PreviousModeIsAuto == true) || FirstCycle))
                {
                    Console.WriteLine("Manual triggering Mode");
                    log.Debug("Manual triggering Mode");
                    PreviousModeIsManual = true;
                    PreviousModeIsMaintenance = false;
                    PreviousModeIsAuto = false;
                    Mode_Cycles_Indeicator = Mode_Cycles_Indeicator + 4;
                }
                else if (message.Contains("Manual triggering") && !((PreviousModeIsAuto == true) || FirstCycle))
                {
                    if (!PreviousModeIsManual)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Manual Mode not after auto mode: " + Mode_Transitions_Counter.ToString());
                        Console.WriteLine("Mnt: {0}, Auto: {1}, Man: {2}", PreviousModeIsMaintenance, PreviousModeIsAuto, PreviousModeIsManual);
                        Console.ResetColor();
                        //Console.WriteLine("\n\r");
                        log.Error("Manual Mode not after auto mode: " + Mode_Transitions_Counter.ToString());
                        Mode_Cycles_Indeicator = -1;
                        Mode_Transitions_Error_Counter++;
                    }
                    else
                    {
                        Console.WriteLine("Duplicate manual.");
                        log.Error("Duplicate manual line entry.");
                    }
                }
                if (Mode_Cycles_Indeicator == 7)
                {
                    Mode_Cycles_Indeicator = 0;
                    Mode_Transitions_Counter++;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Modes Cycle completed: " + Mode_Transitions_Counter.ToString());
                    log.Debug("Modes Cycle completed: " + Mode_Transitions_Counter.ToString());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Modes Cycle Errors: " + Mode_Transitions_Error_Counter.ToString());
                    log.Debug("Modes Cycle Errors: " + Mode_Transitions_Error_Counter.ToString());
                    Console.ResetColor();
                    SendToUI(udpClient, "Modes", Mode_Transitions_Counter, Mode_Transitions_Error_Counter, 0, 0);
                }
                else if (Mode_Cycles_Indeicator == -1)
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Modes Cycle Errors: " + Mode_Transitions_Error_Counter.ToString());
                    Console.ResetColor();
                    log.Debug("Modes Cycle Errors: " + Mode_Transitions_Error_Counter.ToString());
                    Mode_Cycles_Indeicator = 0;
                    SendToUI(udpClient, "Modes", Mode_Transitions_Counter, Mode_Transitions_Error_Counter, 0, 0);
                }
                else if ((Mode_Cycles_Indeicator >= 1) && (Mode_Cycles_Indeicator <= 6))
                {

                    Console.WriteLine("Modes MidCycle");
                    log.Debug("Modes MidCycle");
                }
            }
            if (param.Equals("ArmDisarm"))
            {
                bool StatesFirstCycle = !(PreviousStateIsIdle || PreviousStateIsArmed || PreviousStateIsDisarmed);
                if (message.Contains(" ARMED") && ((PreviousStateIsIdle == true) || StatesFirstCycle))
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine("Armed State, DO NOT TURN OFF SMARTAIR.");
                    Console.ResetColor();
                    log.Debug("SmartAir Armed");
                    PreviousStateIsArmed = true;
                    PreviousStateIsDisarmed = false;
                    PreviousStateIsIdle = false;
                    State_Cycles_Indeicator = State_Cycles_Indeicator + 1;
                }
                else if (message.Contains(" ARMED") && !((PreviousStateIsIdle == true) || StatesFirstCycle))
                {
                    if (!PreviousStateIsArmed)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Armed state not after Idle state: " + State_Transitions_Counter.ToString());
                        Console.WriteLine("Armed: {0}, Idle: {1}, Disarmed: {2}", PreviousStateIsArmed, PreviousStateIsIdle, PreviousStateIsDisarmed);
                        Console.ResetColor();
                        log.Error("Armed state not after Idle state: " + State_Transitions_Counter.ToString());
                        State_Cycles_Indeicator = -1;
                        State_Transitions_Error_Counter++;
                    }
                    else
                    {
                        Console.WriteLine("Duplicate Armed.");
                        log.Error("Duplicate Armed line entry.");
                    }
                }

                if (message.Contains("DISARMED") && ((PreviousStateIsArmed == true) || StatesFirstCycle))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("It is safe to power off the SmartAir.");
                    Console.ResetColor();
                    Console.WriteLine("Disarmed State");
                    log.Debug("Disarmed state");
                    PreviousStateIsArmed = false;
                    PreviousStateIsDisarmed = true;
                    PreviousStateIsIdle = false;
                    State_Cycles_Indeicator = State_Cycles_Indeicator + 2;
                }
                else if (message.Contains("DISARMED") && !((PreviousStateIsArmed == true) || StatesFirstCycle))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("It is safe to power off the SmartAir.");
                    Console.ResetColor();
                    if (!PreviousStateIsDisarmed)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Disarmed state not after Armed state: " + State_Transitions_Counter.ToString());
                        Console.WriteLine("Armed: {0}, Idle: {1}, Disarmed: {2}", PreviousStateIsArmed, PreviousStateIsIdle, PreviousStateIsDisarmed);
                        Console.ResetColor();
                        log.Error("Disarmed state not after Armed state: " + State_Transitions_Counter.ToString());
                        State_Cycles_Indeicator = -1;
                        State_Transitions_Error_Counter++;
                    }
                    else
                    {
                        Console.WriteLine("Duplicate Disarmed.");
                        log.Error("Duplicate Disarmed line entry.");
                    }
                }

                if (message.Contains("IDLE") && ((PreviousStateIsDisarmed == true) || StatesFirstCycle))
                {
                    Console.WriteLine("Idle state");
                    log.Debug("Idle state");
                    PreviousStateIsArmed = false;
                    PreviousStateIsDisarmed = false;
                    PreviousStateIsIdle = true;
                    State_Cycles_Indeicator = State_Cycles_Indeicator + 4;
                }
                else if (message.Contains("IDLE") && !((PreviousStateIsDisarmed == true) || StatesFirstCycle))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("It is safe to power off the SmartAir.");
                    Console.ResetColor();
                    if (!PreviousStateIsIdle)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Idle state not after Disarmed state: " + State_Transitions_Counter.ToString());
                        Console.WriteLine("Armed: {0}, Idle: {1}, Disarmed: {2}", PreviousStateIsArmed, PreviousStateIsIdle, PreviousStateIsDisarmed);
                        Console.ResetColor();
                        //Console.WriteLine("\n\r");
                        log.Error("Idle state not after Disarmed state: " + State_Transitions_Counter.ToString());
                        State_Cycles_Indeicator = -1;
                        State_Transitions_Error_Counter++;
                    }
                    else
                    {
                        Console.WriteLine("Duplicate Idle.");
                        log.Error("Duplicate Idle line entry.");
                    }
                }
                if (State_Cycles_Indeicator == 7)
                {
                    State_Cycles_Indeicator = 0;
                    State_Transitions_Counter++;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("State Cycle completed: " + State_Transitions_Counter.ToString());
                    log.Debug("State Cycle completed: " + State_Transitions_Counter.ToString());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("State Cycle Errors: " + State_Transitions_Error_Counter.ToString());
                    log.Debug("State Cycle Errors: " + State_Transitions_Error_Counter.ToString());
                    Console.ResetColor();
                    SendToUI(udpClient, "ArmDisarm", State_Transitions_Counter, State_Transitions_Error_Counter, 0, 0);
                }
                else if (State_Cycles_Indeicator == -1)
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("State Cycle Errors: " + State_Transitions_Error_Counter.ToString());
                    Console.ResetColor();
                    log.Debug("State Cycle Errors: " + State_Transitions_Error_Counter.ToString());
                    State_Cycles_Indeicator = 0;
                    SendToUI(udpClient, "ArmDisarm", State_Transitions_Counter, State_Transitions_Error_Counter, 0, 0);
                }
                else if ((State_Cycles_Indeicator >= 1) && (State_Cycles_Indeicator <= 6))
                {

                    Console.WriteLine("State MidCycle");
                    log.Debug("State MidCycle");
                }
            }
            if (param.Equals("Discharge"))
            {
                if (message.Contains(": Finished successfully"))
                {
                    log.Debug("SmartAir finished initialization.");
                    Thread.Sleep(5000);
                    _serialPort.WriteLine("atg\r");
                    log.Debug("Arm command.");
                }
                if (message.Contains("RC ARM/DISARM was Triggered"))
                {

                    log.Debug("SmartAir is armed");
                    Thread.Sleep(5000);
                    log.Debug("Disconnect Capacitor");
                    //TODO: Send to Arduino command to disconnect relay
                    ArduinoPort.WriteLine("Dis");
                    Thread.Sleep(10000);
                    log.Debug("Reconnect Capacitor");
                    //TODO: Send to Arduino command to reconnect relay
                    ArduinoPort.WriteLine("Rec");
                    discharge_Counter++;
                    SendToUI(udpClient, "Discharge", discharge_Counter, 0, 0, 0);
                }
            }
            if (param.Equals("XBTTest"))
            {
                if (message.Contains(": Finished successfully"))
                {
                    log.Debug("SmartAir finished initialization.");
                    Thread.Sleep(5000);
                    _serialPort.WriteLine("atg\r");
                    log.Debug("Arm command.");
                }
                if (message.Contains("RC ARM/DISARM was Triggered"))
                {

                    log.Debug("SmartAir is armed");
                    Thread.Sleep(5000);
                    log.Debug("Set XBT to Low");
                    ArduinoPort.WriteLine("XBTLow");
                    //stopWatch = new Stopwatch();
                    stopWatch.Start();
                    //Thread.Sleep(5000);
                }
                if (message.Contains(": DISARMED"))
                {
                    TimeSpan ts = stopWatch.Elapsed;
                    log.Debug("SmartAir is disarmed");
                    Thread.Sleep(1000);
                    ArduinoPort.WriteLine("XBTHigh");
                    if (ts.TotalMilliseconds < 5500)
                    {
                        XBT_disarm_Counter++;
                    }
                    else
                    {
                        XBT_disarm_Counter_Error++;
                    }
                    SendToUI(udpClient, "XBTTest", XBT_disarm_Counter, XBT_disarm_Counter_Error, 0, 0);
                    _serialPort.WriteLine("rst\r");
                }
            }
            if (param.Equals("XBTTestPowerUp"))
            {
                if (message.Contains(": Finished successfully"))
                {
                    log.Debug("SmartAir finished initialization.");
                    Thread.Sleep(3000);
                    log.Debug("Set XBT to Low");
                    ArduinoPort.WriteLine("XBTLow");
                    Thread.Sleep(3000);
                    _serialPort.WriteLine("rst\r");
                }

                if (message.Contains("!XBT LOW signal detected. Waiting for signal off"))
                {
                    log.Debug("Wait 45 seconds");
                    Thread.Sleep(45000);
                    string messageLeftOver = _serialPort.ReadExisting();//TODO: Check if it is need to access through message
                    if (!messageLeftOver.Contains(": Finished successfully"))
                    {
                        XBT_disarm_Counter++;
                    }
                    else
                    {
                        XBT_disarm_Counter_Error++;
                    }
                    SendToUI(udpClient, "XBTTestPowerUp", XBT_disarm_Counter, XBT_disarm_Counter_Error, 0, 0);
                    ArduinoPort.WriteLine("XBTHigh");
                    Thread.Sleep(5000);
                    _serialPort.WriteLine("rst\r");
                }
            }
            if (param.Equals("ArmAtStartUp"))
            {
                if (message.Contains(": Finished successfully"))
                {
                    log.Debug("SmartAir finished initialization.");
                    Thread.Sleep(3000);
                    Successful_Init_Counter++;
                    _serialPort.WriteLine("\r");
                    _serialPort.WriteLine("rst\r");
                }

                if (message.Contains("System.....................: ARMED"))
                {
                    Successful_Init_Counter_Error++;
                    _serialPort.WriteLine("\r");
                    _serialPort.WriteLine("rst\r");
                }
                SendToUI(udpClient, "ArmAtStartUp", Successful_Init_Counter, Successful_Init_Counter_Error, 0, 0);
            }
            if (param.Equals("TriggerDueToRC"))
            {
                //stopWatch = new Stopwatch();
                if (message.Contains(": Finished successfully"))
                {
                    log.Debug("SmartAir finished initialization.");
                    Thread.Sleep(1000);
                    ArmUsingArduino();
                    resetStopWatch.Start();
                }

                if (message.Contains("System.....................: ARMED"))
                {
                    FireUsingArduino();
                }

                if (message.Contains("MOTOR_OFF"))
                {
                    Console.WriteLine("Reset in 5 Seconds");
                    log.Debug("Reset SmartAir in 5 second due to Trigger detection");
                    General_Counter++;
                    Thread.Sleep(5000);
                    WriteToSmartAir("rst");
                }
                TimeSpan ts = stopWatch.Elapsed;
                if (ts.TotalMilliseconds >= 25000)
                {
                    General_Counter_Error++;
                    WriteToSmartAir("rst");
                }

                SendToUI(udpClient, "TriggerDueToRC", General_Counter, General_Counter_Error, 0, 0);
            }
            if (param.Equals("DisarmDueToNoVib"))
            {
                //stopWatch = new Stopwatch();
                if (message.Contains(": Finished successfully"))
                {
                    log.Debug("SmartAir finished initialization.");
                    Thread.Sleep(1000);
                    ArmUsingArduino();
                    resetStopWatch.Start();
                }

                if (message.Contains("due to no vibrations"))
                {
                    Console.WriteLine("Disarmed due to no vibrations");
                    log.Debug("Disarmed due to no vibrations");
                    General_Counter++;
                    Thread.Sleep(1000);
                    WriteToSmartAir("rst");
                }
                TimeSpan ts = stopWatch.Elapsed;
                if (ts.TotalMilliseconds >= 15000)
                {
                    General_Counter_Error++;
                    WriteToSmartAir("rst");
                }

                SendToUI(udpClient, "DisarmDueToNoVib", General_Counter, General_Counter_Error, 0, 0);
            }
            if (param.Equals("TestAutoPort"))
            {
                General_Counter++;
                Thread.Sleep(1000);
                if (message.Contains(": Finished successfully"))
                {
                    Console.WriteLine("Reset in 5 Seconds");
                    log.Debug("Reset SmartAir in 5 seconds due to Motor off detection");
                    General_Counter++;
                    Thread.Sleep(5000);
                    WriteToSmartAir("rst");
                }
                SendToUI(udpClient, "TestAutoPort", General_Counter, General_Counter_Error, 0, 0);
            }
            if (param.Equals("PWMToRelay"))
            {
                int LongTest = 0;
                int ServoPWMLength = 0;
                int MotorPWMLength = 0;
                //stopWatch = new Stopwatch();
                if (!stopWatch.IsRunning)
                    stopWatch.Start();
                //WriteToSmartAir("rst");
                //WaitForSuccessfulInit();
                if (message.Contains(": Finished successfully"))
                {
                    String TimeNow = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
                    WriteToSmartAir("DTM " + "\"" + TimeNow + "\"");
                    log.Debug("SmartAir finished initialization.");
                    WaitForReset = false;
                    WriteToArduino("PWMREADServo");
                    FullTextArduino = "";
                    Thread.Sleep(1000);
                    ServoPWMLength = PWMLengthConvertor();
                    string TempArduinoText = FullTextArduino;
                    WriteToArduino("PWMREADMotor");
                    FullTextArduino = "";
                    Thread.Sleep(1000);
                    MotorPWMLength = PWMLengthConvertor();
                    if ((ServoPWMLength < 1050) && (MotorPWMLength < 1050))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("PWM Length at idle passed.");
                        Console.ResetColor();
                        log.Debug("PWM Length at idle passed.");
                    }
                    else
                    {
                        PWM_width_error_Counter++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("PWM Length at idle failed. #:" + PWM_width_error_Counter.ToString());
                        Console.ResetColor();
                        log.Error("PWM Length at idle failed. #:" + PWM_width_error_Counter.ToString());
                        log.Error("Servo PWM:");
                        log.Error(TempArduinoText);
                        log.Error("Motor PWM:");
                        log.Error(FullTextArduino);

                    }
                    FullTextSmartAir = "";
                    TempArduinoText = "";
                    WriteToSmartAir("atg", "!System.....................: ARMED", true, 3);
                    //WaitForText("!System.....................: ARMED");
                    WriteToArduino("PWMREADServo");
                    FullTextArduino = "";
                    Thread.Sleep(1000);
                    ServoPWMLength = PWMLengthConvertor();
                    TempArduinoText = FullTextArduino;
                    WriteToArduino("PWMREADMotor");
                    FullTextArduino = "";
                    Thread.Sleep(1000);
                    MotorPWMLength = PWMLengthConvertor();

                    if ((ServoPWMLength < 1050) && (MotorPWMLength < 1050))
                    {
                        PWM_width_Counter++;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("PWM Length after arm passed. #:" + PWM_width_Counter.ToString());
                        Console.ResetColor();
                        log.Debug("PWM Length after arm passed. #:" + PWM_width_Counter.ToString());
                    }
                    else
                    {
                        PWM_width_error_Counter++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("PWM Length after arm failed. #:" + PWM_width_error_Counter.ToString());
                        Console.ResetColor();
                        log.Error("PWM Length after arm failed. #:" + PWM_width_error_Counter.ToString());
                        log.Error("Servo PWM:");
                        log.Error(TempArduinoText);
                        log.Error("Motor PWM:");
                        log.Error(FullTextArduino);
                    }
                    FullTextSmartAir = "";
                    //WriteToSmartAir("fire");
                    WriteToSmartAir("fire", "!RC was Trigger PYRO.", true, 2);
                    //WaitForText("SWITCH MOTOR_OFF");
                    WriteToArduino("PWMREADServo");
                    FullTextArduino = "";
                    Thread.Sleep(1000);
                    stopWatch.Restart();
                }
                if (message.Contains("!RC was Trigger PYRO."))
                {
                    Console.WriteLine("Reset in 15 Seconds");
                    log.Debug("Reset SmartAir in 15 seconds ");
                    ServoPWMLength = PWMLengthConvertor();
                    string TempArduinoText = FullTextArduino;
                    WriteToArduino("PWMREADMotor");
                    FullTextArduino = "";
                    Thread.Sleep(1000);
                    MotorPWMLength = PWMLengthConvertor();
                    if ((ServoPWMLength > 1850) && (MotorPWMLength > 1850))
                    {
                        General_Counter++;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("PWM Length after trigger passed. #:" + General_Counter.ToString());
                        Console.ResetColor();
                        log.Debug("PWM Length after trigger passed. #:" + General_Counter.ToString());
                    }
                    else
                    {
                        General_Counter_Error++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("PWM Length after trigger failed. #:" + General_Counter_Error.ToString());
                        Console.ResetColor();
                        log.Error("PWM Length after trigger failed. #:" + General_Counter_Error.ToString());
                        log.Error("Servo PWM:");
                        log.Error(TempArduinoText);
                        log.Error("Motor PWM:");
                        log.Error(FullTextArduino);
                    }

                    ColoerdTimer(10000);
                    WriteToSmartAir("rst", "!Application................: Start", true, 3);
                    stopWatch.Reset();
                }
                ts = stopWatch.Elapsed;
                if ((ts.TotalMilliseconds >= TestTimeOutDuration) && (!WaitForReset))
                {
                    LongTest++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test Did not finish within 35 seconds. #:" + LongTest.ToString());
                    Console.ResetColor();
                    log.Error("Test Did not finish within 35 seconds. #:" + LongTest.ToString());
                    WriteToSmartAir("rst", "!Application................: Start", true, 3);
                    WaitForReset = true;
                }
                SendToUI(udpClient, "PWMToRelay", PWM_width_Counter, PWM_width_error_Counter, General_Counter, General_Counter_Error);
            }
            if (param.Equals("PWMToRelaySoftReset"))
            {
                int LongTest = 0;
                bool TrueOrFalse = false;
                //stopWatch = new Stopwatch();
                //WriteToSmartAir("rst");
                //WaitForSuccessfulInit();
                if (!stopWatch.IsRunning)
                    stopWatch.Start();
                if (message.Contains(": Finished successfully"))
                {
                    String TimeNow = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
                    WriteToSmartAir("DTM " + "\"" + TimeNow + "\"");
                    log.Debug("SmartAir finished initialization.");
                    WaitForReset = false;
                    WriteToSmartAir("rst");
                    FullTextArduino = "";
                    WaitForSuccessfulInit();
                    TrueOrFalse = FullStringPWMLengthConvertor(1050, false);
                    if (TrueOrFalse)
                    {
                        PWM_width_Counter++;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("PWM Length at soft reset passed. #:" + PWM_width_Counter.ToString());
                        Console.ResetColor();
                        log.Debug("PWM Length at soft reset passed. #:" + PWM_width_Counter.ToString());
                    }
                    else
                    {
                        PWM_width_error_Counter++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("PWM Length at soft reset failed. #:" + PWM_width_error_Counter.ToString());
                        Console.ResetColor();
                        log.Error("PWM Length at soft reset failed. #:" + PWM_width_error_Counter.ToString());
                    }
                    Thread.Sleep(2000);
                    stopWatch.Restart();
                }
                ts = stopWatch.Elapsed;
                if (ts.TotalMilliseconds >= TestTimeOutDuration && (!WaitForReset))
                {
                    WaitForReset = true;
                    LongTest++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test Did not finish within 15 seconds. #:" + LongTest.ToString());
                    Console.ResetColor();
                    log.Error("Test Did not finish within 15 seconds. #:" + LongTest.ToString());
                    WriteToSmartAir("rst");

                }
                SendToUI(udpClient, "PWMToRelaySoftReset", PWM_width_Counter, PWM_width_error_Counter, 0, 0);
            }
            if (param.Equals("PWMVoltageAtPowerCycle"))
            {

                bool TrueOrFalse = false;
                if (!stopWatch.IsRunning)
                    stopWatch.Start();
                if (FullTextSmartAir.Contains(": Finished successfully"))
                {
                    String TimeNow = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
                    WriteToSmartAir("DTM " + "\"" + TimeNow + "\"");
                    WaitForReset = false;
                    WriteToArduino("PWMREADServoStop");
                    FullTextSmartAir = "";
                    log.Debug("SmartAir finished initialization.");

                    TrueOrFalse = FullStringPWMLengthConvertor(1050, false);

                    if (TrueOrFalse)
                    {

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("PWM Length at hard reset passed. #:" + PWM_width_Counter.ToString());
                        Console.ResetColor();
                        log.Debug("PWM Length at hard reset passed. #:" + PWM_width_Counter.ToString());
                    }
                    else
                    {
                        PWM_width_error_Counter++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("PWM Length at hard reset failed. #:" + PWM_width_error_Counter.ToString());
                        Console.ResetColor();
                        log.Error("PWM Length at hard reset failed. #:" + PWM_width_error_Counter.ToString());
                    }
                    Thread.Sleep(2000);
                    FullTextArduino = "";
                    WriteToArduino("TSTVLTG");
                    int VoltIndex = FullTextArduino.IndexOf("Voltage #");
                    if (!VoltIndex.Equals(-1))
                    {
                        TrueOrFalse = FullStringVoltageConvertor(0.6, false);
                        //double VoltVal = Convert.ToDouble(FullTextArduino.Substring(VoltIndex + 9, 3));
                        if (TrueOrFalse)
                        {

                        }
                        else
                        {
                            General_Counter_Error++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Relay Voltage during hard power up trigger failed. #:" + General_Counter_Error.ToString());
                            Console.ResetColor();
                            log.Error("Relay Voltage during hard power up failed. " + FullTextArduino);
                            log.Error("Relay Voltage during hard power up failed. #:" + General_Counter_Error.ToString());
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Arduino Failed, Skipped Cycle");
                        Console.ResetColor();
                        log.Error("Relay Voltage during hard power up failed. " + FullTextArduino);
                        log.Error("Arduino Failed, Skipped Cycle");
                    }
                    WriteToArduino("PWMREADServo");
                    WriteToSmartAir("atg");
                    WriteToSmartAir("fire");
                    stopWatch.Restart();
                }
                if (message.Contains("MOTOR OFF PWM LOW") || message.Contains("PYRO PWM LOW"))
                {
                    Console.WriteLine("Reset in 25 Seconds");
                    log.Debug("Reset SmartAir in 25 seconds ");
                    FullTextArduino = "";
                    WriteToArduino("TSTVLTG");
                    int VoltIndex = FullTextArduino.IndexOf("Voltage #");
                    if (!VoltIndex.Equals(-1))
                    {
                        //double VoltVal = Convert.ToDouble(FullTextArduino.Substring(VoltIndex + 9, 3));
                        TrueOrFalse = FullStringVoltageConvertor(4.6, true);
                        if (TrueOrFalse)
                        {
                            General_Counter++;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Relay Voltage after trigger passed. #:" + General_Counter.ToString());
                            Console.ResetColor();
                            log.Debug("Relay Voltage after trigger passed. #:" + General_Counter.ToString());
                        }
                        else
                        {
                            General_Counter_Error++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Relay Voltage after trigger failed. #:" + General_Counter_Error.ToString());
                            Console.ResetColor();
                            log.Error("Relay Voltage after trigger failed. " + FullTextArduino);
                            log.Error("Relay Voltage after trigger failed. #:" + General_Counter_Error.ToString());
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Arduino Failed, Skipped Cycle");
                        Console.ResetColor();
                        log.Error("Relay Voltage after trigger failed. " + FullTextArduino);
                        log.Error("Arduino Failed, Skipped Cycle");
                    }

                    ColoerdTimer(18000);
                    FullTextArduino = "";
                    WriteToArduino("PWRDWN");
                    WriteToArduino("PWMREADServo");

                    TrueOrFalse = FullStringPWMLengthConvertor(1050, false);
                    if (TrueOrFalse)
                    {
                        PWM_width_Counter++;
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("PWM Length at hard reset passed. #:" + PWM_width_Counter.ToString());
                        Console.ResetColor();
                        log.Debug("PWM Length at hard reset passed. #:" + PWM_width_Counter.ToString());
                    }
                    else
                    {
                        PWM_width_error_Counter++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("PWM Length at hard reset failed. #:" + PWM_width_error_Counter.ToString());
                        Console.ResetColor();
                        log.Error("PWM Length at hard reset failed. #:" + PWM_width_error_Counter.ToString());
                    }
                    WriteToArduino("PWMREADServoStop");
                    Thread.Sleep(2000);
                    WriteToArduino("PWRUP");
                    Thread.Sleep(5000);
                    WriteToArduino("PWMREADServo");
                    if (!_serialPort.IsOpen)
                        try
                        {
                            _serialPort.Open();
                        }
                        catch (Exception ex)
                        {
                            log.Error("SmartAir Port failed.");
                            log.Error(ex);
                            if (ex.Message.Contains("Access to the port is denied"))
                            {
                                message = "";
                                ResetSmartAir();
                            }
                            else
                            {
                                WriteToArduino("PWMREADServoStop");
                                WriteToArduino("PWRDWN");
                                ColoerdTimer(3000);
                                WriteToArduino("PWRUP");
                            }
                        }
                    stopWatch.Restart();
                }
                ts = stopWatch.Elapsed;
                if (ts.TotalMilliseconds >= TestTimeOutDuration && (!WaitForReset))
                {
                    LongTest++;
                    WaitForReset = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test Did not finish within 35 seconds. #:" + LongTest.ToString());
                    Console.ResetColor();
                    log.Error("Test Did not finish within 35 seconds. #:" + LongTest.ToString());
                    WriteToSmartAir("rst");//, "Reseting SmartAir Nano...",true,2);
                    stopWatch.Restart();
                    ts = ts.Subtract(ts);
                }
                else if (ts.TotalMilliseconds >= 120000)
                {
                    LongTest++;
                    WaitForReset = true;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test Did not finish within 120 seconds. #:" + LongTest.ToString());
                    Console.ResetColor();
                    log.Error("Test Did not finish within 120 seconds. #:" + LongTest.ToString());
                    WriteToArduino("PWRDWN");
                    ColoerdTimer(5000);
                    WriteToArduino("PWRUP");
                    stopWatch.Restart();
                    ts = ts.Subtract(ts);
                    try
                    {
                        if (!_serialPort.IsOpen)
                        {
                            _serialPort.Open();
                        }
                    }
                    catch (Exception Ex)
                    {
                        log.Error("Com Port Does not exists????"); //does not exist
                        log.Error(Ex);
                    }
                }
                SendToUI(udpClient, "PWMVoltageAtPowerCycle", PWM_width_Counter, PWM_width_error_Counter, General_Counter, General_Counter_Error);
            }
            if (param.Equals("Q10Voltage"))
            {
                int LongTest = 0;
                int ServoPWMLength = 0;
                int MotorPWMLength = 0;
                //stopWatch = new Stopwatch();
                if (!stopWatch.IsRunning)
                    stopWatch.Start();
                //WriteToSmartAir("rst");
                //WaitForSuccessfulInit();
                if (message.Contains(": Finished successfully"))
                {
                    String TimeNow = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
                    WriteToSmartAir("DTM " + "\"" + TimeNow + "\"");
                    log.Debug("SmartAir finished initialization.");
                    WaitForReset = false;
                    FullTextArduino = "";
                    WriteToArduino("A1VLTG");
                    Thread.Sleep(1000);

                    if (FullTextArduino.Contains("Voltage #"))
                    {
                        if (Convert.ToDouble(FullTextArduino.Substring(FullTextArduino.IndexOf("Voltage #") + 9, 4)) <= 0.9)
                        {
                            //PWM_width_Counter++;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Q10 Voltage at idle passed.");
                            Console.ResetColor();
                            log.Debug("Q10 Voltage at idle passed.");
                        }
                        else
                        {
                            PWM_width_error_Counter++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Q10 Voltage at idle failed. #:" + PWM_width_error_Counter.ToString());
                            Console.ResetColor();
                            log.Error("Q10 Voltage at idle failed. #:" + PWM_width_error_Counter.ToString());
                            log.Error(FullTextArduino);

                        }
                    }
                    FullTextSmartAir = "";
                    WriteToSmartAir("atg", "!System.....................: ARMED", true, 3);

                    FullTextSmartAir = "";
                    WriteToSmartAir("fire", "!RC was Trigger PYRO.", true, 2);

                    FullTextArduino = "";
                    WriteToArduino("A1VLTG");
                    Thread.Sleep(1000);
                    stopWatch.Restart();
                }
                if (message.Contains("!RC was Trigger PYRO."))
                {
                    Console.WriteLine("Reset in 15 Seconds");
                    log.Debug("Reset SmartAir in 15 seconds ");
                    if (FullTextArduino.Contains("Voltage #"))
                    {
                        if (Convert.ToDouble(FullTextArduino.Substring(FullTextArduino.IndexOf("Voltage #") + 9, 4)) >= 3.9)
                        {
                            General_Counter++;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Q10 Voltage after trigger passed.");
                            Console.ResetColor();
                            log.Debug("Q10 Voltage after trigger passed.");
                        }
                        else
                        {
                            General_Counter_Error++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Q10 Voltage after trigger failed. #:" + General_Counter_Error.ToString());
                            Console.ResetColor();
                            log.Error("Q10 Voltage after trigger failed. #:" + General_Counter_Error.ToString());
                            log.Error(FullTextArduino);
                        }
                    }

                    ColoerdTimer(15000);

                    FullTextArduino = "";
                    WriteToArduino("A1VLTG");
                    Thread.Sleep(1000);

                    if (FullTextArduino.Contains("Voltage #"))
                    {
                        if (Convert.ToDouble(FullTextArduino.Substring(FullTextArduino.IndexOf("Voltage #") + 9, 4)) <= 0.9)
                        {
                            PWM_width_Counter++;
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Q10 Voltage 20 seconds after trigger passed.");
                            Console.ResetColor();
                            log.Debug("Q10 Voltage 20 seconds after trigger passed.");
                        }
                        else
                        {
                            PWM_width_error_Counter++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Q10 Voltage 20 seconds after trigger failed. #:" + PWM_width_error_Counter.ToString());
                            Console.ResetColor();
                            log.Error("Q10 Voltage 20 seconds after trigger failed. #:" + PWM_width_error_Counter.ToString());
                            log.Error(FullTextArduino);

                        }
                    }

                    WriteToSmartAir("rst", "!Application................: Start", true, 3);
                    stopWatch.Reset();
                }
                ts = stopWatch.Elapsed;
                if ((ts.TotalMilliseconds >= TestTimeOutDuration) && (!WaitForReset))
                {
                    LongTest++;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Test Did not finish within 35 seconds. #:" + LongTest.ToString());
                    Console.ResetColor();
                    log.Error("Test Did not finish within 35 seconds. #:" + LongTest.ToString());
                    WriteToSmartAir("rst", "!Application................: Start", true, 3);
                    WaitForReset = true;
                }
                SendToUI(udpClient, "Q10Voltage", PWM_width_Counter, PWM_width_error_Counter, General_Counter, General_Counter_Error);
            }
        }
    }

    private static void SendToUI(UdpClient udpClient, String testName, long Value1, long Error1, long Value2, long Error2)
    {
        try
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(testName + ": Value:" + Value1.ToString("D9") + " Error: " + Error1.ToString("D9")
                + " Value2: " + Value2.ToString("D9") + " Error2: " +
                Error2.ToString("D9") + " PID:" + PID.ToString() + " EOL");
            udpClient.Send(sendBytes, sendBytes.Length);
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error encoding message.");
            Console.WriteLine("Test name: {0}, Value1: {1}, Error1: {2}, Value2: {3}, Error2: {4}", testName, Value1, Error1, Value2, Error2);
            log.ErrorFormat("Test name: {0}, Value1: {1}, Error1: {2}, Value2: {3}, Error2: {4}", testName, Value1, Error1, Value2, Error2);
            Console.ResetColor();
        }

    }

    // Display Port values and prompt user to enter a port.
    public static string SetPortName(string defaultPortName)
    {
        string portName;

        Console.WriteLine("Available Ports:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
        portName = Console.ReadLine();

        if (portName == "" || !(portName.ToLower()).StartsWith("com"))
        {
            portName = defaultPortName;
        }
        return portName;
    }

    // Display BaudRate values and prompt user to enter a value.
    public static int SetPortBaudRate(int defaultPortBaudRate)
    {
        string baudRate;

        Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
        baudRate = Console.ReadLine();

        if (baudRate == "")
        {
            baudRate = defaultPortBaudRate.ToString();
        }

        return int.Parse(baudRate);
    }

    // Display PortParity values and prompt user to enter a value.
    public static Parity SetPortParity(Parity defaultPortParity)
    {
        string parity;

        Console.WriteLine("Available Parity options:");
        foreach (string s in Enum.GetNames(typeof(Parity)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
        parity = Console.ReadLine();

        if (parity == "")
        {
            parity = defaultPortParity.ToString();
        }

        return (Parity)Enum.Parse(typeof(Parity), parity, true);
    }

    // Display DataBits values and prompt user to enter a value.
    public static int SetPortDataBits(int defaultPortDataBits)
    {
        string dataBits;

        Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
        dataBits = Console.ReadLine();

        if (dataBits == "")
        {
            dataBits = defaultPortDataBits.ToString();
        }

        return int.Parse(dataBits.ToUpperInvariant());
    }

    // Display StopBits values and prompt user to enter a value.
    public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits;

        Console.WriteLine("Available StopBits options:");
        foreach (string s in Enum.GetNames(typeof(StopBits)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter StopBits value (None is not supported and \n" +
         "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
        stopBits = Console.ReadLine();

        if (stopBits == "")
        {
            stopBits = defaultPortStopBits.ToString();
        }

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }

    public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake;

        Console.WriteLine("Available Handshake options:");
        foreach (string s in Enum.GetNames(typeof(Handshake)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
        handshake = Console.ReadLine();

        if (handshake == "")
        {
            handshake = defaultPortHandshake.ToString();
        }

        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }

    static void ArduinoPortInitialization()
    {
        ArduinoPort.PortName = ArduinoCOMPort;
        ArduinoPort.BaudRate = 115200;
        ArduinoPort.Parity = 0;
        ArduinoPort.DataBits = 8;
        ArduinoPort.StopBits = (StopBits)1;
        ArduinoPort.Handshake = 0;
        ArduinoPort.ReadBufferSize = 9000000;
        ArduinoPort.Open();

        ArduinoPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedFromArduinoHandler);
    }

    private static void UpdatelogFournetfile()
    {
        using (FileStream fs = File.Open(CurrDir + "\\ConsoleSerialPortReader.exe.log4net", FileMode.Create, FileAccess.Write, FileShare.None))
        {
            string Part1 = "<?xml version=\"1.0\" encoding=\"utf - 8\" ?> \r\n < log4net > \r\n < root > \r\n < level value = \"ALL\" /> " +
                "<appender -ref ref= \"MyAppender\" /> < appender -ref ref= \"RollingFileAppender\" /> </ root >" +
                "< appender name = \"MyFileAppender\" type = \"log4net.Appender.FileAppender\" >" +
                "< file value = \"application.log\" /> < appendToFile value = \"true\" />" +
                "< lockingModel type = \"log4net.Appender.FileAppender+MinimalLock\" />" +
                "< layout type = \"log4net.Layout.PatternLayout\" > < conversionPattern value = \"%date %level %logger - %message%newline\" />" +
                "</ layout > </ appender > < appender name = \"RollingFileAppender\" type = \"log4net.Appender.RollingFileAppender\" >" +
                "< file type = \"log4net.Util.PatternString\" value = \"";
            string Part2 = CurrDir;
            string Part3 = "\\logs/Motor-%utcdate{yyyy-MM-dd-hh-mm-ss}.log\" /> < rollingStyle value = \"Date\" /> " +
                "< datePattern value = \"yyyyMMdd\" /> < appendToFile value = \"false\" /> < rollingStyle value = \"Size\" /> " +
                "< maxSizeRollBackups value = \"50\" /> < maximumFileSize value = \"100MB\" /> < staticLogFileName value = \"false\" /> " +
                "< layout type = \"log4net.Layout.PatternLayout\" > < conversionPattern value = \"%date [%thread] %level %logger - %message%newline\" /> " +
                "</ layout > </ appender > </ log4net > ";

            Byte[] info = new UTF8Encoding(true).GetBytes(Part1 + Part2 + Part3);
            // Add some information to the file.
            fs.Write(info, 0, info.Length);
            fs.Close();
        }
    }

    class CWDFileAppender : FileAppender
    {
        public override string File
        {
            set
            {
                base.File = Path.Combine(Directory.GetCurrentDirectory(), value);
            }
        }
    }

    static void ArmUsingArduino()
    {
        FullTextArduino = "";
        ArduinoPort.WriteLine("ARM");
        WaitForSuccessfulArmCommand();
        Thread.Sleep(SleepAfterDisarmEvent);
    }

    static void ModeChangeUsingArduino()
    {
        FullTextArduino = "";
        ArduinoPort.WriteLine("Mode");
        WaitForSuccessfulModeCommand();
        Thread.Sleep(SleepAfterDisarmEvent);
    }

    static void FireUsingArduino()
    {
        FullTextArduino = "";
        ArduinoPort.WriteLine("FIRE");
        WaitForSuccessfulFireCommand();
        Thread.Sleep(SleepAfterDisarmEvent);
    }

    static void WaitForSuccessfulInit()
    {
        WaitForInit = true;
        Stopwatch resetStopWatch = new Stopwatch();
        resetStopWatch.Start();
        //Thread.Sleep(250);
        TimeSpan ts = resetStopWatch.Elapsed;
        while ((WaitForInit) && ts.TotalMilliseconds <= 35000)
        {
            ts = resetStopWatch.Elapsed;
            if (FullTextSmartAir.Contains(": Finished successfully") ||
                (FullTextSmartAir.Contains("!System.....................: IDLE") && !FullTextSmartAir.Contains("!IMU Sensor.................: Version: 0.00")))
                WaitForInit = false;
        }
        Thread.Sleep(SleepDurationAfterBoardInitMethod);
    }

    static void WaitForSemiSuccessfulInit()
    {
        WaitForInit = true;
        //Thread.Sleep(250);
        while (WaitForInit)
        {
            if (FullTextSmartAir.Contains(": Finished successfully")
                || FullTextSmartAir.Contains("--Initialization--"))
                WaitForInit = false;
        }
    }

    static void WaitForSuccessfulArmCommand()
    {
        WaitForInit = true;
        //Thread.Sleep(250);
        while (WaitForInit)
        {
            if (FullTextArduino.Contains("ARMEnded"))
                WaitForInit = false;
        }
        //FullTextArduino = "";
        //LogTestToFile(CurrentClass, "Arm Executed\r");
    }

    static void WaitForSuccessfulFireCommand()
    {
        WaitForInit = true;
        //Thread.Sleep(250);
        while (WaitForInit)
        {
            if (FullTextArduino.Contains("FIREEnded"))
                WaitForInit = false;
        }
        //FullTextArduino = "";
        //LogTestToFile(CurrentClass, "Fire Executed\r");
    }

    static void WaitForSuccessfulModeCommand()
    {
        WaitForInit = true;
        //Thread.Sleep(250);
        while (WaitForInit)
        {
            if (FullTextArduino.Contains("Mode Sequence Ended"))
                WaitForInit = false;
        }
        //
        //LogTestToFile(CurrentClass, "Mode Sequence Ended\r");
    }

    /*static void WaitForText(string TextToSearch)
    {
        WaitForInit = true;
        Stopwatch resetStopWatch = new Stopwatch();
        resetStopWatch.Start();
        //Thread.Sleep(250);
        TimeSpan ts = resetStopWatch.Elapsed;
        while ((WaitForInit) && ts.TotalMilliseconds <= 35000)
        {
            ts = resetStopWatch.Elapsed;
            if (FullTextSmartAir.Contains(TextToSearch))
                WaitForInit = false;
            if (_serialPort.BytesToRead>0)
            {
                message = _serialPort.ReadExisting();
                FullTextSmartAir += message; 
            }
        }
        if (WaitForInit)
        {
            Console.WriteLine("###SmartAir Did not Reply.");
            log.Error("###SmartAir Did not Reply.");
        }
        Thread.Sleep(SleepDurationAfterBoardInitMethod);
    }*/

    static bool WaitForText(string TextToSearch)
    {
        bool WaitForText = true;
        Stopwatch resetStopWatch = new Stopwatch();
        resetStopWatch.Start();

        TimeSpan ts = resetStopWatch.Elapsed;
        while ((WaitForText) && (ts.TotalMilliseconds <= 35000))
        {
            ts = resetStopWatch.Elapsed;
            if (FullTextSmartAir.Contains(TextToSearch))
                WaitForText = false;
            if (_serialPort.BytesToRead > 0)
            {
                message = _serialPort.ReadExisting();
                FullTextSmartAir += message;
            }
        }
        if (WaitForText)
        {
            Console.WriteLine("###SmartAir Did not Reply.");
            log.Error("###SmartAir Did not Reply.");
        }
        Thread.Sleep(250);
        return WaitForText;
    }

    static bool WaitForText(string TextToSearch, int TimeOutTimerDurationInMilliSec)
    {
        bool WaitForText = true;
        Stopwatch resetStopWatch = new Stopwatch();
        resetStopWatch.Start();

        TimeSpan ts = resetStopWatch.Elapsed;
        while ((WaitForText) && (ts.TotalMilliseconds <= TimeOutTimerDurationInMilliSec))
        {
            ts = resetStopWatch.Elapsed;
            if (FullTextSmartAir.Contains(TextToSearch))
                WaitForText = false;
            if (_serialPort.BytesToRead > 0)
            {
                message = _serialPort.ReadExisting();
                FullTextSmartAir += message;
            }
        }
        if (WaitForText)
        {
            Console.WriteLine("###SmartAir Did not Reply.");
            log.Error("###SmartAir Did not Reply.");
        }
        Thread.Sleep(250);
        return WaitForText;
    }

    static void DataReceivedFromArduinoHandler(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();
        if (!indata.Equals(""))
        {
            FullTextArduino += indata;
        }
    }

    static void WriteToSmartAir(string TextToSend)
    {
        try
        {
            if (_serialPort.IsOpen)
            {
                Console.WriteLine("Command sent to SmartAir: " + TextToSend);
                _serialPort.WriteLine("\r");
                _serialPort.WriteLine(TextToSend + "\r");
                Thread.Sleep(SleepAfterWriteLineEvent);
            }
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                Console.WriteLine("Command sent to SmartAir: " + TextToSend);
                _serialPort.WriteLine("\r");
                _serialPort.WriteLine(TextToSend + "\r");
                Thread.Sleep(SleepAfterWriteLineEvent);
            }
        }
        catch (Exception ex)
        {
            log.Error("SmartAir Port failed.");
            log.Error(ex);
            if (ex.Message.Contains("Access to the port is denied"))
            {
                message = "";
                ResetSmartAir();
            }
            else
            {
                WriteToArduino("PWMREADServoStop");
                WriteToArduino("PWRDWN");
                ColoerdTimer(3000);
                WriteToArduino("PWRUP");
            }

        }

    }

    static bool WriteToSmartAir(string TextToSend, string ReplayToSearchFor, bool Retry, int RetryCounter)
    {
        bool TextFound = false;
        int Count = 0;
        try
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine("\r");
                _serialPort.WriteLine(TextToSend + "\r");
                Thread.Sleep(SleepAfterWriteLineEvent);
            }
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                _serialPort.WriteLine("\r");
                _serialPort.WriteLine(TextToSend + "\r");
                Thread.Sleep(SleepAfterWriteLineEvent);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Access to the port is denied"))
            {
                _serialPort.Close();
                Thread.Sleep(SleepAfterWriteLineEvent);
                _serialPort.Open();
            }
            else
            {

            }

        }
        TextFound = !WaitForText(ReplayToSearchFor);
        if (TextFound)
        {

        }
        while ((!TextFound) && Retry && Count < RetryCounter)
        {
            Count++;
            WriteToSmartAir(TextToSend, ReplayToSearchFor, true, 0);
        }
        return TextFound;
    }

    static bool WriteToSmartAir(string TextToSend, string ReplayToSearchFor, bool Retry, int RetryCounter, int TimeOutDuration)
    {
        bool TextFound = false;
        int Count = 0;
        try
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.WriteLine("\r");
                _serialPort.WriteLine(TextToSend + "\r");
                Thread.Sleep(SleepAfterWriteLineEvent);
            }
            if (!_serialPort.IsOpen)
            {
                _serialPort.Open();
                _serialPort.WriteLine("\r");
                _serialPort.WriteLine(TextToSend + "\r");
                Thread.Sleep(SleepAfterWriteLineEvent);
            }
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Access to the port is denied"))
            {
                _serialPort.Close();
                Thread.Sleep(SleepAfterWriteLineEvent);
                _serialPort.Open();
            }
            else
            {

            }

        }
        TextFound = !WaitForText(ReplayToSearchFor, TimeOutDuration);
        if (TextFound)
        {

        }
        while ((!TextFound) && Retry && Count < RetryCounter)
        {
            Count++;
            WriteToSmartAir(TextToSend, ReplayToSearchFor, true, 0, TimeOutDuration);
        }
        return TextFound;
    }

    static int PWMLengthConvertor()
    {
        int PWMSignalLength = 0;
        int PWMLengthIndex = FullTextArduino.IndexOf("PWM Pulse Width:");
        try
        {
            PWMSignalLength = Convert.ToInt16(FullTextArduino.Substring(PWMLengthIndex + 16, 5));
        }
        catch
        {

        }
        return PWMSignalLength;
    }

    static bool FullStringPWMLengthConvertor(int PWMValue, bool SmallerOrGreater)
    {
        int PWMSignalLength = 0;
        bool ConditionMet = true;
        int Count = 0;
        string LocalText = "";
        LocalText = FullTextArduino;
        int PWMLengthIndex = 0;
        while ((LocalText.Contains("PWM Pulse Width: ")) && (LocalText.Length > 50) && (LocalText.IndexOf("PWM Pulse Width: ") < 30))
        {
            try
            {
                PWMLengthIndex = LocalText.IndexOf("PWM Pulse Width:");
                PWMSignalLength = Convert.ToInt16(LocalText.Substring(PWMLengthIndex + 16, 5));
                LocalText = LocalText.Remove(0, PWMLengthIndex + 16 + 5);
                if (!SmallerOrGreater)
                {
                    if ((PWMSignalLength > PWMValue) && (ConditionMet))
                    {
                        ConditionMet = false;
                    }
                }
                if (SmallerOrGreater)
                {
                    if ((PWMSignalLength < PWMValue) && (ConditionMet))
                    {
                        ConditionMet = false;
                    }
                }
            }
            catch
            {

            }
        }
        return ConditionMet;
    }

    static void WaitForSuccessfulArduinoCommand(string TextToFind)
    {
        int j = 0;
        Stopwatch LocalSW = new Stopwatch();
        TimeSpan LocalTS;
        LocalSW.Start();
        WaitForInit = true;
        Console.WriteLine("@@@Started waiting for Arduino");
        //Thread.Sleep(250);
        while (WaitForInit)
        {
            if (FullTextArduino.Contains(TextToFind + "Ended"))
                WaitForInit = false;
            LocalTS = LocalSW.Elapsed;
            if (LocalTS.TotalSeconds > 15)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Arduino Failed To Respond.");
                Console.ResetColor();
                log.ErrorFormat("Arduino Failed To Respond.");
                for (j = 0; j < 3; j++)
                {
                    ArduinoPort.WriteLine("PWMREADStop");
                    Thread.Sleep(500);
                }
                break;
            }

        }
        //FullTextArduino = "";
        //LogTestToFile(CurrentClass, "Fire Executed\r");
        Console.WriteLine("@@@Stoped waiting for Arduino");
    }

    static void WriteToArduino(string TextToSend)
    {
        FullTextArduino = "";
        Console.WriteLine("Sending To Arduino: " + TextToSend);
        ArduinoPort.WriteLine(TextToSend);
        WaitForSuccessfulArduinoCommand(TextToSend);
        Thread.Sleep(1000);
    }
    static void ColoerdTimer(int TimeToWaitInMiliSeconds)
    {
        Stopwatch LocalSW = new Stopwatch();
        TimeSpan LocalTS, LastLocalTS;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(TimeToWaitInMiliSeconds + " milliseconds timer started.");
        Console.ResetColor();
        LocalSW.Start();
        LastLocalTS = LocalSW.Elapsed;
        LocalTS = LocalSW.Elapsed;
        while (LocalTS.TotalMilliseconds < TimeToWaitInMiliSeconds)
        {
            Thread.Sleep(100);
            if (LocalTS.TotalMilliseconds - LastLocalTS.TotalMilliseconds > 1000)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(LocalTS.TotalSeconds.ToString() + "\r");
                Console.ResetColor();
                LastLocalTS = LocalTS;
            }
            LocalTS = LocalSW.Elapsed;
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Timer Expired");
        Console.ResetColor();
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

    static bool FullStringVoltageConvertor(double VoltageRequiredValue, bool SmallerOrGreater)
    {
        int VoltageValue = 0;
        bool ConditionMet = true;
        string LocalText = "";
        LocalText = FullTextArduino;
        int VoltageIndex = 0;
        while ((LocalText.Contains("Voltage #")) && (LocalText.Length > 50) && (LocalText.IndexOf("Voltage #") < 30))
        {
            try
            {
                VoltageIndex = LocalText.IndexOf("Voltage #");
                VoltageValue = Convert.ToInt16(LocalText.Substring(VoltageIndex + 9, 3));
                LocalText = LocalText.Remove(0, VoltageIndex + 9 + 3);
                if (!SmallerOrGreater)
                {
                    if ((VoltageValue > VoltageRequiredValue) && (ConditionMet))
                    {
                        ConditionMet = false;
                    }
                }
                if (SmallerOrGreater)
                {
                    if ((VoltageValue < VoltageRequiredValue) && (ConditionMet))
                    {
                        ConditionMet = false;
                    }
                }
            }
            catch
            {

            }
        }
        return ConditionMet;
    }
}