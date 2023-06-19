using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using System.Threading;
using System.IO.Ports;



public class ArduinoCom : MonoBehaviour
{
    private SerialPort _port;
    public string portName = "COM4";
    public int _baudRate = 115200;
    

    Thread _threadEMG;
    private bool _isThreadEMGRunning;

    int max_vector_size = 10;
    private float lastValue;
    float value_to_send;

    private int pos = 0, bufferSize = 0;
    private int[] buffer = new int[1000];



    public void OpenPort()
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
                Debug.Log("Error: " + e);
            }
        }
    }


    public void Start()  
    {
        // Define the port 
        _port = new SerialPort("COM4", 115200);


        if (!_isThreadEMGRunning)
        {
            OpenPort();
            if (_port.IsOpen)
            {
                _threadEMG = new Thread(ReadData);
                _isThreadEMGRunning = true;
               _threadEMG.Start();
                Debug.Log("Receiving data..");
            }
        }

        // Test
        //StartCoroutine("TestData");
    }
/*
    private IEnumerator TestData()
    {
        while (_isThreadEMGRunning)
        {
            Debug.Log(value_to_send);
            yield return new WaitForSeconds(1f); // wait for 0.1 seconds
            
        }
    }
*/
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
        value_to_send = lastValue;

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
            Debug.Log("Error while sending data" + e);
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


    private void Update()
    {
        //Debug.Log(value_to_send);
    }

    private void OnApplicationQuit()
    {
        Stop();
        // _port.Close();
    }



}
