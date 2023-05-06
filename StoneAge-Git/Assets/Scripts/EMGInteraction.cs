using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EMGInteraction : MonoBehaviour
{

    XRGrabInteractable grabbable;

    private XRDirectInteractor handInteractor;
    private GameObject rightHandController;
    private GameObject leftHandController;

    public GameObject closedrighthand;
    private GameObject closedrighthandinstance;
    public GameObject closedlefthand;
    private GameObject closedlefthandinstance;

    HandComplete righthandcomplete;
    HandComplete lefthandcomplete;

    public enum Hand { Right, Left };
    public Hand CurrentHand { get; private set; }

    public string portName = "COM3";
    public int baudRate = 921600;

    private SerialPort serial;
    private bool isReading = false;
    private bool feedbackState = false;

    public int EMGThreshold = 15;


    private void Start()
    {
        grabbable = GameObject.Find("Club").GetComponent<XRGrabInteractable>();

        rightHandController = GameObject.Find("RightHand Controller");
        leftHandController = GameObject.Find("LeftHand Controller");

        righthandcomplete = rightHandController.GetComponent<HandComplete>();
        lefthandcomplete = leftHandController.GetComponent<HandComplete>();


        SetRightHand();


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

            if (serial.IsOpen)
            {


                float input = Serial_Data_Reading();

                Debug.Log(input);

                if (input>EMGThreshold)
                {
                    HandleFlex();
                    if (!feedbackState)
                    {
                        serial.Write("1");
                        Invoke("turnoffvibration", 0.05f);
                        feedbackState = true;

                    }
                }
                else if (input<300)
                {
                    HandleRelaxed();
                    feedbackState = false;
                    
                }



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

            if (CurrentHand == Hand.Right)
            {
                
                righthandcomplete.HideHandOnSelectEMG();
            }
            else if (CurrentHand == Hand.Left)
            {
                
                lefthandcomplete.HideHandOnSelectEMG();
            }

            
            
        }
    
    }


    private void HandleRelaxed()
    {
        if (handInteractor.isSelectActive)
        {
            handInteractor.EndManualInteraction();

            if (CurrentHand == Hand.Right)
            {
                
                righthandcomplete.HideHandOnDeSelectEMG();
            }
            else if (CurrentHand == Hand.Left)
            {
                
                lefthandcomplete.HideHandOnDeSelectEMG();
            }

            
           

        }

       
    }


    private void OnApplicationQuit()
    {
        isReading = false;
    }
    private void turnoffvibration()
    {
        serial.Write("0");


    }

    #region Seperate settings for Right & Left Hands

    public void SetRightHand()
    {
        handInteractor = rightHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Right;
        Debug.Log("RightHandSelected");
    }

    public void SetLeftHand()
    {
        handInteractor = leftHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Left;
        Debug.Log("LeftHandSelected");
    }



    private void InstantiateRightHandClosed()
    {
        // Instantiate the prefab
        closedrighthandinstance = Instantiate(closedrighthand, rightHandController.transform);
    }

    private void DestroyRightHandClosed()
    {
        // Destroy the prefab
        Destroy(closedrighthandinstance);
    }

    private void InstantiateLeftHandClosed()
    {
        // Instantiate the prefab
        closedlefthandinstance = Instantiate(closedlefthand, leftHandController.transform);
    }

    private void DestroyLeftHandClosed()
    {
        // Destroy the prefab
        Destroy(closedlefthandinstance);
    }

    #endregion


}