using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EMGInteraction : MonoBehaviour
{

    XRGrabInteractable grabbable;
    //private XRGrabInteractable grabbable;
    // [SerializeField] private XRInteractionManager IM;

    private XRDirectInteractor handInteractor;
    private GameObject rightHandController;
    private GameObject leftHandController;

    public enum Hand { Right, Left };
    public Hand CurrentHand { get; private set; }

    public string portName = "COM3";
    public int baudRate = 921600;

    private SerialPort serial;
    private bool isReading = false;
    private bool feedbackState = false;


    private float time = 0.05f;

    

    private void Start()
    {
        grabbable = GameObject.Find("Club").GetComponent<XRGrabInteractable>();
        // grabbable = clubGrabbable;
        rightHandController = GameObject.Find("RightHand Controller");
        leftHandController = GameObject.Find("LeftHand Controller");

        handInteractor = leftHandController.GetComponent<XRDirectInteractor>();

        StopAllCoroutines();
        serial = new SerialPort(portName, baudRate);
        serial.Open();
        StartCoroutine(SerialReadCoroutine());

        InvokeRepeating("Serial_Data_Reading", 0f, 0.3f);

        

        serial.Write("0");

    }

    float Serial_Data_Reading()
    {
        float rcdData = float.Parse(serial.ReadLine());
        return rcdData;
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
                //string input = serial.ReadLine();

                float input = Serial_Data_Reading();

                Debug.Log(input);

                if (input>300)
                {
                    HandleFlex();
                    if (!feedbackState)
                    {
                        serial.Write("1");
                        Invoke("turnoffvibration", time);
                        feedbackState = true;

                    }
                }
                else if (input<300)
                {
                    HandleRelaxed();
                    feedbackState = false;
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

            yield return new WaitForSeconds(0.3f);
        }

        serial.Close();
    }



    private void HandleFlex()
    {
        if(!handInteractor.isSelectActive)
        {
            handInteractor.StartManualInteraction(grabbable);
        }
        

        Animator animator = handInteractor.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Grip", 1.0f);
            Debug.Log("Hand animation played");
        }
    }

    private void HandleRelaxed()
    {
        if (handInteractor.isSelectActive)
        {
            handInteractor.EndManualInteraction();
        }

        Animator animator = handInteractor.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("Grip", 0.0f);
            Debug.Log("Hand animation played");
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

    private void turnoffvibration()
    {
        serial.Write("0");
        

    }
}