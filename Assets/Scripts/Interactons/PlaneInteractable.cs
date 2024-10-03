using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneInteractable : Interactable
{
    [SerializeField] GameObject inPlaneUi;

    [SerializeField] AirplaneController airplaneController;

    [SerializeField] CinemachineVirtualCamera planeCamera;

    [SerializeField] GameObject player;

    [SerializeField] private Transform playerExitPosition;

    private bool isPlayerInPlane = false;

    [SerializeField] private Rigidbody planeRigidbody; 


    private void Start()
    {
        planeCamera.gameObject.SetActive(false);
    }

    public override void OnFocus()
    {
        if (!isPlayerInPlane)
        {
            interactionText = "Press 'F' to enter the plane";
        }
    }

    public override void OnInteract()
    {
        EnterPlane();

    }

    public override void OnLoseFocus()
    {
        if (!isPlayerInPlane)
        {
            interactionText = string.Empty;
        }
    }
     
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPlane();
        }
    }

    private void EnterPlane()
    {
        interactionText = "Press 'Escape' to exit the plane";

        inPlaneUi.SetActive(true);

        // Hide the player.
        player.SetActive(false);

        // Enable the plane's controls.
        airplaneController.ActivateControls();

        // Activate the plane camera.
        planeCamera.gameObject.SetActive(true);

        isPlayerInPlane = true;

        Debug.Log("Player has entered the plane.");
    }

    // Method to handle the player exiting the plane.
    private void ExitPlane()
    {
        inPlaneUi.SetActive(false);

        // Deactivate the plane's controls.
        airplaneController.ActivateControls();

        // Disable the plane camera.
        planeCamera.gameObject.SetActive(false);

        // Show the player and move them to the designated exit position.
        player.SetActive(true);
        player.transform.position = playerExitPosition.position;

        // Reactivate player controls here if needed (e.g., enabling character controller, input, etc.)

        isPlayerInPlane = false;

        Debug.Log("Player has exited the plane.");
    }

    private bool IsPlaneStationary()
    {
        return planeRigidbody.velocity.magnitude < 0.1f && planeRigidbody.angularVelocity.magnitude < 0.1f;
    }
}
