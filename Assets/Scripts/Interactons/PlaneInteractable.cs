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

    public bool needsPlayer = true;

    private void Start()
    {
        if (planeCamera != null && needsPlayer)
        {
            planeCamera.gameObject.SetActive(false);
        }
        if (!needsPlayer)
        {
            inPlaneUi.SetActive(true);
            airplaneController.ActivateControls();
            planeCamera.gameObject.SetActive(true);
            isPlayerInPlane = true;
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
        GameManager.Instance.OnPlayerEnterPlane.Invoke();

        GameManager.Instance.DisablePlayerControls(false);
        Cursor.visible = false;

        inPlaneUi.SetActive(true);
        player.SetActive(false);

        airplaneController.ActivateControls();
        planeCamera.gameObject.SetActive(true);

        isPlayerInPlane = true;

        GameManager.Instance.PlayerInPlane(isPlayerInPlane);
        InteractionHandler.Instance.UpdateInteractionText("Press 'Escape' to exit the plane");
        //Debug.Log("Player has entered the plane.");
    }

    private void ExitPlane()
    {
        player.SetActive(true);
        inPlaneUi.SetActive(false);
        airplaneController.ActivateControls();
        planeCamera.gameObject.SetActive(false);

        player.transform.position = playerExitPosition.position;
        isPlayerInPlane = false;
        GameManager.Instance.PlayerInPlane(isPlayerInPlane);
        GameManager.Instance.EnablePlayerControls();

        Debug.Log("Player has exited the plane.");
    }

    private bool IsPlaneStationary()
    {
        return planeRigidbody.velocity.magnitude < 0.1f && planeRigidbody.angularVelocity.magnitude < 0.1f;
    }
}
