using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
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

    public ArduinoCom _arduino;

    public enum Hand { Right, Left };
    public Hand CurrentHand { get; private set; }


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


        turnoffvibration();

        Debug.Log("_arduino is " + (_arduino == null ? "not " : "") + "assigned.");

    }

    //private IEnumerator SerialReadCoroutine()

    private void FixedUpdate()
    {

        float input = _arduino.GetData();
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

    #endregion

    public float GetAvgValue()
    {
        List<float> data = new List<float>();
        float elapsedTime = 0f;
        float averageData = 0f;

        while (elapsedTime < 3f)
        {
            float input = _arduino.GetData();
            data.Add(input);
            elapsedTime += Time.deltaTime;
        }

        if (data.Count > 0)
        {
            averageData = data.Average();
        }

        Debug.Log("Average Value While Flexing = " + averageData);
        EMGThreshold = (int) averageData;
        return averageData;
    }

}