using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PageCategory
{
    contents,
    ores,
    islands,
    enemies,
    plane,
    quests,

}


public class Page : MonoBehaviour
{
    public PageCategory pageCategory;

    //[SerializeField] private List<GameObject> pages = new List<GameObject>();
    [SerializeField] List<Section> sections = new List<Section>();

    public static Action<String> OnOreMined;

    private void Start()
    {
        Debug.Log("subscribed to onOreMined");
       
    }

    private void OnEnable()
    {
        OnOreMined += ActivateSection;
    }

    private void OnDisable()
    {
        OnOreMined -= ActivateSection;
    }

    public void ActivateSection(String name)
    {
        Debug.Log("ActivateSection called for: " + name);
        Debug.Log("Sections count: " + sections.Count);

        foreach (Section section in sections)
        {
            if (section.sectionName.ToString() == name /*&& !section.gameObject.activeSelf*/)
            {
                section.gameObject.SetActive(true);
                Debug.Log($"Activated section: {section.sectionName}");
            }
        }
    }
}
