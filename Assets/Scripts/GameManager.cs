using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }


    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject virtualMouseUI;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject airplane;

    private AirplaneAerodynamics airplaneAerodynamics;
    private CharacterMovement playerController;
    private InventorySystem inventorySystem;

    private bool isPLayerInPlane = false;

    public UnityAction OnPlayerEnterPlane;
    public UnityAction OnPlayerExitPlane;

    private void Awake()
    {
        Instance = this;
        if (player == null || airplane == null)
        {
            Debug.Log("No player or airplane object attached to the Game Manager ");
            return;
        }
        if (player != null && airplane != null)
        {
            playerController = player.GetComponent<CharacterMovement>();
            inventorySystem = player.GetComponent<InventorySystem>();
            airplaneAerodynamics = airplane.GetComponent<AirplaneAerodynamics>();
        }
    }

    public void DisablePlayerControls(bool showVirtualMouse)
    {
        //Debug.Log("Should hide crosshair");
        if (virtualMouseUI != null && showVirtualMouse)
        {
            virtualMouseUI.SetActive(true);
        }
        crosshair.SetActive(false);
        playerController?.DisableControls();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnablePlayerControls()
    {
        if (virtualMouseUI != null)
        {
            virtualMouseUI.SetActive(false);
        }
        crosshair.SetActive(true);
        playerController?.EnableControls();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public InventorySystem GetInventorySystem()
    {

        return inventorySystem;
    }

    public AirplaneAerodynamics GetAirplaneAerodynamics()
    {
        return airplaneAerodynamics;
    }

    public void PlayerInPlane(bool state)
    {
        isPLayerInPlane = state;
    }

    public bool IsPLayerInPlane()
    {
        return isPLayerInPlane;
    }
}
