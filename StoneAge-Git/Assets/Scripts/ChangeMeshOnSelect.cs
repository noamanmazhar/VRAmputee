using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeMeshOnSelect : MonoBehaviour
{
    public Mesh replacementMesh;

    private MeshFilter meshFilter;
    private Mesh originalMesh;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
    }

    private void OnEnable()
    {
        GetComponent<XRGrabInteractable>().onSelectEntered.AddListener(OnSelectEntered);
        GetComponent<XRGrabInteractable>().onSelectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        GetComponent<XRGrabInteractable>().onSelectEntered.RemoveListener(OnSelectEntered);
        GetComponent<XRGrabInteractable>().onSelectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(XRBaseInteractor interactor)
    {
        meshFilter.mesh = replacementMesh;
    }

    private void OnSelectExited(XRBaseInteractor interactor)
    {
        meshFilter.mesh = originalMesh;
    }
}