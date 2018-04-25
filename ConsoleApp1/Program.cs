// Use this code inside a project created with the Visual C# > Windows Desktop > Console Application template.
// Replace the code in Program.cs with this code.

using log4net.Appender;
using log4net.Config;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class PortChat
{
    static bool _continue;
    static SerialPort _serialPort;
    static string param;
    static bool detected_Motor_signal_on;
    static long trigger_with_Motor_High_Counter;
    static long error_Counter;

    //private static readonly log4net.ILog log =
    //        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    //private static readonly CWDFileAppender wwww = new CWDFileAppender();

    private static readonly log4net.ILog log =
        log4net.LogManager.GetLogger(typeof(PortChat));


    public static void Main(string[] args)
    {
        trigger_with_Motor_High_Counter = 0;
        error_Counter = 0;
        param = args[0];
        log.Debug(param);
        detected_Motor_signal_on = false;
        string message;
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);
        FileInfo qqq = new System.IO.FileInfo(Directory.GetCurrentDirectory());
        log.Debug(qqq.ToString());
        // Create a new SerialPort object with default settings.
        _serialPort = new SerialPort();
        // Allow the user to set the appropriate properties.
        _serialPort.PortName = SetPortName("COM6");
        _serialPort.BaudRate = SetPortBaudRate(115200);
        _serialPort.Parity = SetPortParity(_serialPort.Parity);
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);
        log.Debug("Port Data: COM ID: " + _serialPort.PortName);

        // Set the read/write timeouts
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;

        _serialPort.Open();
        _continue = true;
        readThread.Start();

        Console.Write("Port is Open");
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
            }
        }

        readThread.Join();
        _serialPort.Close();
    }

    public static void Read()
    {
        while (_continue)
        {
            try
            {
                string message = _serialPort.ReadLine();
                log.Debug(message);
                Console.WriteLine(message);
                if (param.Contains("Motor"))
                {
                    if (message.Contains("MOTOR_OFF"))
                    {
                        Console.WriteLine("Reset in 1 Second");
                        log.Debug("Reset SmartAir in 1 second due to Motor off detection");
                        Thread.Sleep(1000);
                        _serialPort.Write("RST\r\n");
                    }
                }
                if (param.Contains("MotorSignalDetection"))
                {
                    if (message.Contains("Motor Signal High") && !detected_Motor_signal_on)
                    {
                        Console.WriteLine("Detected Motor Signal High");
                        log.Debug("Detected Motor Signal High");
                        detected_Motor_signal_on = true;
                    }
                    if (message.Contains("<1,1,") && detected_Motor_signal_on)
                    {
                        trigger_with_Motor_High_Counter++;
                        Console.WriteLine("Arm- Trigger Cycle Counter: " + trigger_with_Motor_High_Counter.ToString());
                        log.Debug("SmartAir Indicated triggering after Motor High signal - Count: " + trigger_with_Motor_High_Counter.ToString());
                        detected_Motor_signal_on = false;
                    }
                    else if (message.Contains("<") && !message.Contains("<1,1,") && detected_Motor_signal_on)
                    {
                        error_Counter++;
                        Console.WriteLine("Error Cycle Counter: " + error_Counter.ToString());
                        log.Debug("Error cycle (motor signal without Trigger) - Count: " + error_Counter.ToString());
                        detected_Motor_signal_on = false;
                    }
                }
            }
            catch (TimeoutException) { }
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