using UnityEngine;
using System;
using System.IO.Ports;
using System.Globalization;

public class ArduinoReader : MonoBehaviour
{
    [Serializable]
    public class FlowSensorData
    {
        public int pulses;
        public float flowMSec;
        public float totalL;
    }

    [Header("Serial Settings")]
    public string portName = "COM3";
    public int baudRate = 9600;

    [Header("Connection")]
    public bool isConnected;
    public string rawLine;
    public int timeMs;

    [Header("Flow Sensor Data")]
    public FlowSensorData sensor1 = new FlowSensorData();
    public FlowSensorData sensor2 = new FlowSensorData();

    [Header("Control Data")]
    [Range(1, 2)]
    public int selectedSensorForControl = 1;

    public bool HasNewData { get; private set; }
    public float LatestSensorValue { get; private set; }

    private SerialPort serialPort;

    void Start()
    {
        OpenSerialPort();
    }

    void Update()
    {
        ReadSerialData();
    }

    void OpenSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 50;
            serialPort.WriteTimeout = 100;
            serialPort.Open();

            isConnected = true;
            Debug.Log("Arduino connected on " + portName);
        }
        catch (Exception e)
        {
            isConnected = false;
            Debug.LogError("Could not open serial port: " + e.Message);
        }
    }

    void ReadSerialData()
    {
        if (serialPort == null || !serialPort.IsOpen)
            return;

        try
        {
            string line = serialPort.ReadLine();
            ParseSerialLine(line);
        }
        catch (TimeoutException)
        {
            //nothing needed
        }
        catch (Exception e)
        {
            Debug.LogWarning("Serial read error: " + e.Message);
        }
    }

    void ParseSerialLine(string line)
    {
        line = line.Trim();
        rawLine = line;

        if (line.StartsWith("time_ms"))
            return;

        string[] parts = line.Split(',');

        // Expected Arduino format:
        // time_ms,s1_pulses,s1_flow_M_sec,s1_total_L,s2_pulses,s2_flow_M_sec,s2_total_L

        if (parts.Length != 7)
        {
            Debug.LogWarning("Wrong data format: " + line);
            return;
        }

        int.TryParse(parts[0], out timeMs);

        int.TryParse(parts[1], out sensor1.pulses);
        float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out sensor1.flowMSec);
        float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out sensor1.totalL);

        int.TryParse(parts[4], out sensor2.pulses);
        float.TryParse(parts[5], NumberStyles.Float, CultureInfo.InvariantCulture, out sensor2.flowMSec);
        float.TryParse(parts[6], NumberStyles.Float, CultureInfo.InvariantCulture, out sensor2.totalL);

        if (selectedSensorForControl == 1)
        {
            LatestSensorValue = sensor1.flowMSec;
        }
        else
        {
            LatestSensorValue = sensor2.flowMSec;
        }

        //testing purpose
        System.Random rnd = new System.Random();
        sensor1.flowMSec = rnd.Next(1, 13);
        HasNewData = true;

        Debug.Log(
            "S1 Flow: " + sensor1.flowMSec.ToString("F2") + " m^3/sec | " +
            "S2 Flow: " + sensor2.flowMSec.ToString("F2") + " m^3/sec | " +
            "Selected: " + LatestSensorValue.ToString("F2")
        );
    }

    public void ClearNewDataFlag()
    {
        HasNewData = false;
    }

    public float GetFlowSensor1Value()
    {
        return sensor1.flowMSec;
    }

    public float GetFlowSensor2Value()
    {
        return sensor2.flowMSec;
    }

    public float GetSelectedSensorValue()
    {
        return LatestSensorValue;
    }

    public void SendLineToArduino(string message)
    {
        if (serialPort == null || !serialPort.IsOpen)
        {
            Debug.LogWarning("Cannot send. Serial port is not open.");
            return;
        }

        serialPort.WriteLine(message);
        Debug.Log("Sent to Arduino: " + message);
    }

    void OnApplicationQuit()
    {
        CloseSerialPort();
    }

    void OnDestroy()
    {
        CloseSerialPort();
    }

    void CloseSerialPort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            isConnected = false;
            Debug.Log("Serial port closed.");
        }
    }
}