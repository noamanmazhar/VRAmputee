using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoManager : MonoBehaviour
{
    private string portName = "";
    private SerialPort _port;
    private int _baudRate;
    private float lastValue;
    private int pos = 0, bufferSize = 0;
    private int[] buffer = new int[1000];

    Thread _threadEMG;
    bool _isThreadEMGRunning = false;

    int max_vector_size = 10;
    float value_to_send = 0;

    public ArduinoManager(string port, int baudRate)
    {
        portName = port;
        _baudRate = baudRate;
        _port = new SerialPort(portName, baudRate);
    }

    /// <summary>
    /// Open the Serial Port
    /// </summary>
    public void Open()
    {
        if (!_port.IsOpen)
        {
            try
            {
                _port.Open();
                Debug.Log("Connected..");
            }
            catch (System.Exception e)
            {
                Debug.Log("Port not found: " + portName + "Error:" + e);
            }
        }
    }

    public void Start()
    {
        if (!_isThreadEMGRunning)
        {
            Open();
            if (_port.IsOpen)
            {
                _threadEMG = new Thread(ReadData);
                _isThreadEMGRunning = true;
                _threadEMG.Start();
                Debug.Log("Receiving data..");
            }
        }
    }

    public void Stop()
    {
        if (_isThreadEMGRunning)
        {
            _isThreadEMGRunning = false;
            _threadEMG.Abort();
            _port.Close();
            Debug.Log("Disconnected..");
        }
    }

    private void ReadData()
    {
        while (_isThreadEMGRunning)
        {
            ReadArduinoData();
        }
    }

    private void ReadArduinoData()
    {
        if (!_port.IsOpen)
            return;

        float.TryParse(_port.ReadLine(), out lastValue);

        //Debug.Log("Emg FROM arduino: " + lastValue);
        value_to_send = lastValue;
        //value_to_send = MovingAverage(lastValue); //added moving average calculation
        //buffer[pos] = lastValue * 2;
        //buffer[pos] = lastValue; // remove multiplication by 2
        //pos++; bufferSize++;
    }

    public float MovingAverage(int value)
    {
        try
        {
            //shift elements of the moving average vector
            for (int i = max_vector_size - 1; i > 0; i--)
            {
                buffer[i] = buffer[i - 1];
            }
            buffer[0] = value; // initial position of the vector receives current reading
            float sum = 0;

            for (int i = 0; i < max_vector_size; i++)
            {
                sum += buffer[i];
            }
            return sum / max_vector_size;
        }
        catch (Exception e)
        {
            Debug.Log("Func::MovingAverage::" + e);
            return value_to_send;
        }
    }

    public float GetData()
    {
        return value_to_send;
        //float v = 0;
        //try {
        //    int size = bufferSize;
        //    for (int i = 0; i < size; i++) {
        //        v += buffer[i];
        //        buffer[i] = 0;
        //    }
        //    pos = 0;
        //    bufferSize = 0;
        //    return v / size;
        //} catch (System.Exception e) {
        //    Debug.Log("Error displaying data: " + e);
        //    return 0;
        //}        
    }

    public void SendData(string data)
    {
        try
        {
            if (_port.IsOpen)
            {
                _port.WriteLine(data);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Erro ao enviar dados:" + e);
        }
    }

    public bool PortStatus()
    {
        return _port.IsOpen;
    }

    public bool IsRunning()
    {
        return _isThreadEMGRunning;
    }
}
