using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClubGrabCorountine : MonoBehaviour
{
    [SerializeField] private XRDirectInteractor handInteractor;
    [SerializeField] private XRGrabInteractable grabbable;
    [SerializeField] private XRInteractionManager IM;


    public SerialPort serial = new SerialPort("COM3", 9600);





    void Start()
    {
        serial.Open();
        StartCoroutine(CheckSerialData());

        /*
             try
    {
        serial.Open();
    }
    catch (System.Exception e)
    {
        Debug.LogError("Failed to open serial port: " + e.Message);
        return;
    }
         */
    }

    IEnumerator CheckSerialData()
    {
        while (true)
        {
           // if (serial.IsOpen)
            {
                string data = serial.ReadLine();

                if (data.Contains("Flex"))
                {
                    SelectClub();
                }
                else if (data.Contains("Relaxed"))
                {
                    DeSelectClub();
                }
            }

            yield return new WaitForSeconds(0.1f); // Check for data every 0.1 seconds
        }
    }

    private void SelectClub()
    {
        handInteractor.StartManualInteraction(grabbable);
    }

    private void DeSelectClub()
    {
        handInteractor.EndManualInteraction();
    }


}