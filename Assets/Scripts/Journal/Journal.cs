using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Journal : MonoBehaviour
{
    private ItemInteractable journalItem;

    public static event Action OnJournalOpened;

    private void OnEnable()
    {
        journalItem = GetComponent<ItemInteractable>();
    }


    private void Update()
    {
        if (journalItem != null && journalItem.isHeld && Input.GetKeyDown(KeyCode.J))
        {
            OnJournalOpened?.Invoke();
        }
    }

}
