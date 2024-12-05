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
public class Section : MonoBehaviour
{
    [SerializeField] private Image sectionImage;
    [SerializeField] private string sectionDiscription;


    public Section(Image sectionImage, string sectionDiscription)
    {
        this.sectionImage = sectionImage;
        this.sectionDiscription = sectionDiscription;
    }
}

public class Page : MonoBehaviour
{
    public PageCategory pageCategory;

    //[SerializeField] private List<GameObject> pages = new List<GameObject>();
    List<Section> sections = new List<Section>();

    private bool isActive = false;


    void Start()
    {
    }

    void Update()
    {

    }

    public void UpdatePage()
    {
       foreach(Section section in sections)
        {
            section.gameObject.SetActive(true);
        }
    }


}
