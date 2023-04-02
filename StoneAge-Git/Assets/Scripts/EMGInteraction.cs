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

    private bool lightState = false;
    public float time = 0.05f;

    private void Start()
    {
        serial = new SerialPort(portName, baudRate);
        serial.Open();
        StartCoroutine(SerialReadCoroutine());
        Debug.Log(baudRate);
        handInteractor = rightHandController.GetComponent<XRDirectInteractor>();

        serial.Write("0");

        /* Invoke("HandleFlex", 1.0f);
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
         Invoke("HandleRelaxed", 45.0f);*/

        serial.Write("LEDOFF");

    }

    private void Update()
    {
       
    }

    
    
    private IEnumerator SerialReadCoroutine()
    {
        isReading = true;

        while (isReading)
        {
          //  if (serial.IsOpen && serial.BytesToRead > 0)
                if (serial.IsOpen)
                {
                //string input = serial.ReadLine().Trim();
                string input = serial.ReadLine();
                //Debug.Log(input);
                if (input.Contains("Flex"))
                {
                    HandleFlex();
                    serial.Write("1");
                    Invoke("turnlightoffdelayed" , time);
                }
                else if (input.Contains("Relaxed"))
                {
                    HandleRelaxed();
                    //serial.Write("0");
                }

                /*  switch (input)
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
                */

               
            }

            yield return new WaitForSeconds(0.1f);
        }

        serial.Close();
    }

   

    private void HandleFlex()
    {
        handInteractor.StartManualInteraction(grabbable);
 
        Animator animator = handInteractor.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Grip", 1.0f);
        }
    }

    private void HandleRelaxed()
    {
        handInteractor.EndManualInteraction();

        Animator animator = handInteractor.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Grip", 0.0f);
        }

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

    private void turnlightoffdelayed()
    {
        serial.Write("0");
        lightState = false;

    }
}