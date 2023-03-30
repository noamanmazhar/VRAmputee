using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EMGInteraction : MonoBehaviour
{

    [SerializeField] private XRGrabInteractable grabbable;
    [SerializeField] private XRInteractionManager IM;

    private XRDirectInteractor handInteractor;
    [SerializeField] public GameObject rightHandController;
    [SerializeField] public GameObject leftHandController;

    public enum Hand { Right, Left };
    public Hand CurrentHand { get; private set; }


    public string portName = "COM3";
    public int baudRate = 9600;

    private SerialPort serial;
    private bool isReading = false;
    
    private void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.Open();
        //StartCoroutine(SerialReadCoroutine());
        Debug.Log(baudRate);
        handInteractor = rightHandController.GetComponent<XRDirectInteractor>();
        Invoke("HandleFlex", 1.0f);
        Invoke("HandleRelaxed", 2.0f);
        Invoke("HandleFlex", 5.0f);
        Invoke("HandleRelaxed", 8.0f);
        Invoke("HandleFlex", 15.0f);
        Invoke("HandleRelaxed", 19.0f);
        Invoke("HandleFlex", 21.0f);
        Invoke("HandleRelaxed", 28.0f);
        Invoke("HandleFlex", 30.0f);
        Invoke("HandleRelaxed", 35.0f);
        Invoke("HandleFlex", 41.0f);
        Invoke("HandleRelaxed", 45.0f);

    }

    private void Update()
    {
        // Do your game logic here
    }

    
    
/*    private IEnumerator SerialReadCoroutine()
    {
        isReading = true;

        while (isReading)
        {
          //  if (serial.IsOpen && serial.BytesToRead > 0)
                if (serial.IsOpen)
                {
                //string input = serial.ReadLine().Trim();
                string input = serial.ReadLine();
                Debug.Log(input);
                if (input.Contains("Flex"))
                {
                    HandleFlex();
                }
                else if (input.Contains("Relaxed"))
                {
                    HandleRelaxed();
                }

                *//*  switch (input)
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
                *//*
            }

            yield return new WaitForSeconds(0.1f);
        }

        serial.Close();
    }*/

   

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

    public void SetRightHand()
    {
        handInteractor = rightHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Right;
    }

    public void SetLeftHand()
    {
        handInteractor = leftHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Left;
    }
}