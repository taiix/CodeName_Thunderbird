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
        if (planeCamera != null)
        {
            planeCamera.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("No plane camera found. ");
        }
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
        if (Input.GetKeyDown(KeyCode.Escape) && IsPlaneStationary() && isPlayerInPlane)
        {
            ExitPlane();
        }
    }

    private void EnterPlane()
    {
        GameManager.Instance.DisablePlayerControls();
        Cursor.visible = false;

        inPlaneUi.SetActive(true);
        player.SetActive(false);

        airplaneController.ActivateControls();
        planeCamera.gameObject.SetActive(true);

        isPlayerInPlane = true;

        InteractionHandler.Instance.UpdateInteractionText("Press 'Escape' to exit the plane");
        //Debug.Log("Player has entered the plane.");
    }

    private void ExitPlane()
    {
        inPlaneUi.SetActive(false);
        airplaneController.ActivateControls();
        planeCamera.gameObject.SetActive(false);

        player.SetActive(true);
        player.transform.position = playerExitPosition.position;
        isPlayerInPlane = false;
        GameManager.Instance.EnablePlayerControls();

        Debug.Log("Player has exited the plane.");
    }

    private bool IsPlaneStationary()
    {
        return planeRigidbody.velocity.magnitude < 0.1f && planeRigidbody.angularVelocity.magnitude < 0.1f;
    }
}
