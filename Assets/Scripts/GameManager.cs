using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance {  get; private set; }

    public CharacterMovement playerController;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<CharacterMovement>();
        }
    }

    public void DisablePlayerControls()
    {
        playerController?.DisableControls();
    }

    public void EnablePlayerControls()
    {
        playerController?.EnableControls();
    }
}
