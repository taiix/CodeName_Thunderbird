using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Item", menuName ="ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{

    public string itemName;
    public Sprite itemIcon;

    public int inventorySize;
    public GameObject itemPrefab;


}
