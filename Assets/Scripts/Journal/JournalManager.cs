using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalManager : MonoBehaviour
{

    [SerializeField] private List<Page> pages = new List<Page>();

    [SerializeField] GameObject journalUI;


    private void OnEnable()
    {
        Journal.OnJournalOpened += OpenJournal;
    }

    private void OnDisable()
    {
        Journal.OnJournalOpened -= OpenJournal;
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
            if(page.pageCategory.ToString() == category)
            {
                page.gameObject.SetActive(true);
            }
            else
            {
                page.gameObject.SetActive(false);
            }
        }
    }
}
