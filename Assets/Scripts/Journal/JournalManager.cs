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

    private Page oresPage;

    private void OnEnable()
    {
        journalAction = playerInputs.FindAction("OpenJournal");
        journalAction.performed += OnJournalOpened;

        oresPage = pages.Find(page => page.pageCategory == PageCategory.ores);
        if (oresPage != null)
        {
            Page.OnOreMined += oresPage.ActivateSection;
        }
    }

    private void OnDisable()
    {
        journalAction.performed -= OnJournalOpened;

        if (oresPage != null)
        {
            Page.OnOreMined -= oresPage.ActivateSection;
        }
    }

    void OpenJournal()
    {
        journalUI.SetActive(true);
        GameManager.Instance.DisablePlayerControls(false);
    }

    public void CloseJournal()
    {
        journalUI.SetActive(false);
        GameManager.Instance.EnablePlayerControls(true);
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
            GameManager.Instance.ToggleControns(isOpen);
        }
    }


}
