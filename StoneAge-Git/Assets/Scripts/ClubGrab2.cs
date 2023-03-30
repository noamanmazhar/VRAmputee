using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClubGrab2 : MonoBehaviour
{
    [SerializeField] private XRDirectInteractor handInteractor;
    [SerializeField] private XRGrabInteractable grabbable;
    [SerializeField] private XRInteractionManager IM;

    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort serial;
    private bool isReading = false;

    private void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.Open();
        StartCoroutine(SerialReadCoroutine());
    }

    private void Update()
    {
        // Do your game logic here
    }

    private IEnumerator SerialReadCoroutine()
    {
        isReading = true;

        while (isReading)
        {
            if (serial.IsOpen && serial.BytesToRead > 0)
            {
                string input = serial.ReadLine().Trim();

                switch (input)
                {
                    case "Flex":
                        HandleFlex();
                        break;
                    case "Relaxed":
                        HandleRelaxed();
                        break;
                    default:
                        break;
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        serial.Close();
    }

    private void HandleFlex()
    {
        handInteractor.StartManualInteraction(grabbable);
    }

    private void HandleRelaxed()
    {
        handInteractor.EndManualInteraction();
    }

    private void OnApplicationQuit()
    {
        isReading = false;
    }
}