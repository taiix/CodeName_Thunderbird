using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneInteractable : Interactable
{
    [SerializeField] AirplaneController airplaneController;

    [SerializeField] CinemachineVirtualCamera planeCamera;

    [SerializeField] GameObject player;

    private void Start()
    {
        
    }

    public override void OnFocus()
    {
        interactionText = "Press 'F' to enter the plane";
    }

    public override void OnInteract()
    {
        interactionText = string.Empty;
        airplaneController.ActivateControls();
        player.SetActive(false);
        planeCamera.gameObject.SetActive(true);

    }

    public override void OnLoseFocus()
    {
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            airplaneController.ActivateControls();
            planeCamera.gameObject.SetActive(false);
            player.SetActive(true);
        }
    }
}
