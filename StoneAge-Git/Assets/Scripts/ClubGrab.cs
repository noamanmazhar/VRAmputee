using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClubGrab : MonoBehaviour
{

    [SerializeField] private XRDirectInteractor handInteractor;
    [SerializeField] private XRGrabInteractable grabbable;
    [SerializeField] private XRInteractionManager IM;

    public SerialPort serial = new SerialPort("COM3", 9600);
    private string serialData = "";


    void Start()
    {
       
        serial.Open();
        


    }

    void Update()
    {
        if (serial.IsOpen)
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

