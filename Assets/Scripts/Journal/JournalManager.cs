using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JournalManager : MonoBehaviour
{

    [SerializeField] private List<Page> pages = new List<Page>();
    [SerializeField] GameObject journalUI;
    [SerializeField] InputActionAsset playerInputs;
    [SerializeField] ItemInteractable journal;

    private InputAction journalAction;
    private bool isOpen = false;


    private void OnEnable()
    {
        journalAction = playerInputs.FindAction("OpenJournal");
        journalAction.performed += OnJournalOpened;
    }

    private void OnDisable()
    {
        journalAction.performed -= OnJournalOpened;
    }

    void OpenJournal()
    {
        journalUI.SetActive(true);
        GameManager.Instance.DisablePlayerControls(false);
    }

    public void CloseJournal()
    {
        journalUI.SetActive(false);
        GameManager.Instance.EnablePlayerControls();
    }

    public void OpenPage(string category)
    {
        foreach (Page page in pages)
        {
            if (page.pageCategory.ToString() == category)
            {
                page.gameObject.SetActive(true);
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
    }

    private void OnJournalOpened(InputAction.CallbackContext context)
    {
        //Debug.Log("Journal interacted");
        if (InventorySystem.Instance.HasRequiredItem(journal.itemSO,1))
        {
            isOpen = !isOpen;
            journalUI.SetActive(isOpen);
        }
    }
}
