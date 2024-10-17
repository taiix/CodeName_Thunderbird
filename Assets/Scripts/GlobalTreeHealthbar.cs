using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTreeHealthbar : MonoBehaviour
{
    public static GlobalTreeHealthbar Instance { get; set; }


    public float interactableHealth;
    public float interactableMaxHealth;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
