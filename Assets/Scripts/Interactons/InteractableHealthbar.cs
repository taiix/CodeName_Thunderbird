using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableHealthbar : MonoBehaviour
{
    private Slider slider;

    private float currentHealth;
    private float maxHealth;

    public GlobalTreeHealthbar globalTreeHealthbar;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        currentHealth = globalTreeHealthbar.interactableHealth;
        maxHealth = globalTreeHealthbar.interactableMaxHealth;

        float fillValue = currentHealth / maxHealth;

        slider.value = fillValue;
    }
}
