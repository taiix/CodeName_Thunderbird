using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SectionName
{
    GoldOre,
    PurpleCrystal,
    BlueCrystal,
    OrangeCrystal,

}

public class Section : MonoBehaviour
{
    [SerializeField] private Image sectionImage;
    [SerializeField] private string sectionDiscription;

    public SectionName sectionName;
}
