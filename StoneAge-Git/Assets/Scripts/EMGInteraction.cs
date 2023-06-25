using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

public class EMGInteraction : MonoBehaviour
{

    XRGrabInteractable grabbable;

    private XRDirectInteractor handInteractor;
    private GameObject rightHandController;
    private GameObject leftHandController;


    HandComplete righthandcomplete;
    HandComplete lefthandcomplete;

    [SerializeField] private GameObject CanvasCalib;

    public ArduinoCom _arduino;

   // public InputActionAsset keyboard;
    // public InputAction _keyboardsettings;


    public enum Hand { Right, Left };
    public Hand CurrentHand { get; private set; }


    public bool _EMGDebugLog = false;

    private bool feedbackState = false;

    public int EMGThreshold = 15;
    public float CalibrationDuration = 3f;

    private bool _initialHandSelect = false;


    private void Start()
    {
        grabbable = GameObject.Find("Club").GetComponent<XRGrabInteractable>();

        rightHandController = GameObject.Find("RightHand Controller");
        leftHandController = GameObject.Find("LeftHand Controller");

        righthandcomplete = rightHandController.GetComponent<HandComplete>();
        lefthandcomplete = leftHandController.GetComponent<HandComplete>();

        
        SetRightHand();
        _initialHandSelect = true;


        turnoffvibration();

        // Debug.Log("_arduino is " + (_arduino == null ? "not " : "") + "assigned.");

    }




    private void FixedUpdate()
    {

        float input = _arduino.GetData();
        
        
        if (_EMGDebugLog == true)
        Debug.Log("EMG:\t" + input);

        if (input >= EMGThreshold)
        {
            HandleFlex();
            if (!feedbackState)
            {
                _arduino.SendData("1");

                Invoke("turnoffvibration", 0.05f);
                feedbackState = true;

            }
        }
        else
        {
            HandleRelaxed();
            feedbackState = false;

        }

      
    }

    public float LiveEMGValue(float liveEMG)
    {
        liveEMG = _arduino.GetData();
        return liveEMG;
    }


    private void HandleFlex()
    {
        if (!handInteractor.isSelectActive)
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




    private void turnoffvibration()
    {
        //serial.Write("0");
         _arduino.SendData("0");

    }

    #region Seperate settings for Right & Left Hands

    public void SetRightHand()
    {
        
        if (_initialHandSelect == true)
        {
            handInteractor.EndManualInteraction();
        }

        CanvasCalib.SetActive(true);
        UIText.instance.Right();

        handInteractor = rightHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Right;
        Debug.Log("RightHandSelected");
        Invoke("DisableCanvas", 1f);
    }

    public void SetLeftHand()
    {

        if (_initialHandSelect == true)
        {
            handInteractor.EndManualInteraction();
        }

        CanvasCalib.SetActive(true);
        UIText.instance.Left();

        
        handInteractor = leftHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Left;
        Debug.Log("LeftHandSelected");
        Invoke("DisableCanvas", 1f);
    }

    #endregion

    #region Calibration stuff

    public void GetAvgValue()
    {
        List<float> data = new List<float>();
        float averageData = 0f;

        UIText.instance.Calibrating();
         CanvasCalib.SetActive(true);
        StartCoroutine(CollectDataCoroutine(data, () =>
        {
            if (data.Count > 0)
            {
                averageData = data.Average();
            }

            Debug.Log("Average Value While Flexing = " + averageData);
            EMGThreshold = (int)averageData;
           CanvasCalib.SetActive(false);
        }));
    }

    private IEnumerator CollectDataCoroutine(List<float> data, Action onComplete)
    {
        float elapsedTime = 0f;
        // float CalibrationDuration = 3f;
        float startTime = Time.time;

        while (elapsedTime < CalibrationDuration)
        {
            Debug.Log("Calibrating Flex Value........");
            float input = _arduino.GetData();
            data.Add(input);
            elapsedTime = Time.time - startTime;
            yield return null; // Wait for the next frame
        }

        onComplete?.Invoke(); // Invoke the callback function
    }
    #endregion

    private void DisableCanvas()
    {
        CanvasCalib.SetActive(false);
    }
}