using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {  get; private set; }


    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject airplane;

    private AirplaneAerodynamics airplaneAerodynamics;
    private CharacterMovement playerController;
    private InventorySystem inventorySystem;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
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

    public void DisablePlayerControls()
    {
        crosshair.SetActive(false);
        playerController?.DisableControls();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnablePlayerControls()
    {
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
}
