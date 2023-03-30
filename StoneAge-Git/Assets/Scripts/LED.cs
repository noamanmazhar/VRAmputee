using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class LED : MonoBehaviour
{
    public SerialPort serial = new SerialPort("COM3", 9600);
    private bool lightState = false;
    public float time = 0.05f;


    private void Start()
    {
        serial.Open();
        serial.Write("s");
    }




    private void OnMouseDown()
    {
        if (serial.IsOpen == false)
            serial.Open();

        if (lightState == false)
        {
            serial.Write("A");
            lightState = true;
           // serial.Write("s");
            //lightState = false;
            Invoke("turnlightoffdelayed", time);
            //StartCoroutine(LogLines());

        }
        // else

        /*

            void Start()
    {
        StartCoroutine(LogLines());
    }

    IEnumerator LogLines()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log("Line 1 - Iteration " + (i+1));
            yield return new WaitForSeconds(1.0f);
            Debug.Log("Line 2 - Iteration " + (i+1));
            yield return new WaitForSeconds(1.0f);
        }
    }


         */








        {
            //  serial.Write("B");
            // lightState = false;
        }
    }

    private void turnlightoffdelayed()
    {
        serial.Write("s");
        lightState = false;

    }

    IEnumerator LogLines()
    {
        for (int i = 0; i < 10; i++)
        {
            serial.Write("A");
            lightState = true;
            yield return new WaitForSeconds(0.05f);
            serial.Write("s");
            lightState = false;
            yield return new WaitForSeconds(0.035f);
        }
    }
}
