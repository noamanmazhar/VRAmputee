using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EMGInteraction11 : MonoBehaviour
{
    // For SerialPort
    private readonly string portName = "COM3";
    private readonly int baudRate = 115200;
    private SerialPort serial;
    public string receivedstring;


    // For Vibrational feedback
    private bool feedbackState = false;
    private readonly float time = 0.05f;

    // For VR Interaction
    [SerializeField] private XRGrabInteractable grabbable;
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] public GameObject rightHandController;
    [SerializeField] public GameObject leftHandController;
    private XRDirectInteractor handInteractor;

    // For Hand Selection
    public enum Hand { Left, Right };
    public Hand CurrentHand { get; private set; }

    private void Start()
    {
        handInteractor = leftHandController.GetComponent<XRDirectInteractor>();

        serial = new SerialPort(portName, baudRate);
        
        serial.Open();

        // Ensure Feedback motor is Off initially
        serial.Write("0");

    }

    void FixedUpdate()
    {
        string receivedstring = serial.ReadLine();
        string[] datas = receivedstring.Split(',');

        float rcvValue = float.Parse(datas[0]);

        Debug.Log(rcvValue);

        if (rcvValue > 300)
        {
            HandleFlex();
            if (!feedbackState)
            {
                serial.Write("1");
                Invoke("turnoffvibration", time);
                feedbackState = true;

            }

        }
        else if (rcvValue < 300)
        {
            HandleRelaxed();
            feedbackState = false;
        }
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


    public void SetRightHand()
    {
        handInteractor = rightHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Right;
        Debug.Log("Right Hand Selected");
    }

    public void SetLeftHand()
    {
        handInteractor = leftHandController.GetComponent<XRDirectInteractor>();
        CurrentHand = Hand.Left;
        Debug.Log("Left Hand Selected");
    }

    private void turnoffvibration()
    {
        serial.Write("0");
    }
}