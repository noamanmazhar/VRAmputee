using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClubGrab : MonoBehaviour
{

    [SerializeField] private XRDirectInteractor handInteractor;
    [SerializeField] private XRGrabInteractable grabbable;
    [SerializeField] private XRInteractionManager IM;



    //private XRGrabInteractable currentInteractable = null;

    void Start()
    {
        
        Invoke("SelectClub", 5.0f);
        Invoke("DeSelectClub", 6.0f);
        Invoke("SelectClub", 7.0f);
        Invoke("DeSelectClub", 8.0f);
        Invoke("SelectClub", 9.0f);
        Invoke("DeSelectClub", 10.0f);
        Invoke("SelectClub", 11.0f);
        Invoke("DeSelectClub", 12.0f);


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

