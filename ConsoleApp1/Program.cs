﻿// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
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

    static string message;
    static string CurrDir = "";
    static string LogName = "";

    static string ArduinoCOMPort = "COM3";

    static Stopwatch stopWatch;


    static IPAddress localAddr = IPAddress.Parse("127.0.0.1");

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



        param = args[0];
        if (param.Equals("MotorSignalDetection"))
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
        LogName = param + "_" + DateTime.Now.ToString().Replace("/","-").Replace(" ","_").Replace(":","-") + ".log";
        log4net.GlobalContext.Properties["LogName"] = LogName;
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
            _serialPort.BaudRate = SetPortBaudRate(115200);
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
            _serialPort.BaudRate = 115200;
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

        udpClient.Connect(localAddr, remotePort);

        _serialPort.Open();
        _continue = true;
        readThread.Start();

        Console.WriteLine("Arm - Trigger Tester");
        if (param.Equals("Motor"))
        {
            Console.WriteLine("PSON should be 1000");
        }
        //name = Console.ReadLine();

        Console.WriteLine("Type QUIT to exit");

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
                _serialPort.WriteLine(
                    String.Format("{0}\r\n", message));
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
            }
        }

        readThread.Join();
        _serialPort.Close();
    }

    public static void Read(UdpClient udpClient)
    {
        while (_continue)
        {
            try
            {
                message = _serialPort.ReadLine();
            }
            catch (TimeoutException)
            {
                message = _serialPort.ReadExisting();
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
                    stopWatch = new Stopwatch();
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
                    string messageLeftOver = _serialPort.ReadExisting();
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
        }
    }

    private static void SendToUI(UdpClient udpClient, String testName, long Value1, long Error1, long Value2, long Error2)
    {
        try
        {
            Byte[] sendBytes = Encoding.ASCII.GetBytes(testName + ": Value:" + Value1.ToString("D9") + " Error: " + Error1.ToString("D9")
                + " Value2: " + Value2.ToString("D9") + " Error2: " +
                Error2.ToString("D9") + " EOL");
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

    private static void ArduinoPortInitialization()
    {
        ArduinoPort.PortName = ArduinoCOMPort;
        ArduinoPort.BaudRate = 115200;
        ArduinoPort.Parity = 0;
        ArduinoPort.DataBits = 8;
        ArduinoPort.StopBits = (StopBits)1;
        ArduinoPort.Handshake = 0;
        ArduinoPort.ReadBufferSize = 30000;
        ArduinoPort.Open();
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
}