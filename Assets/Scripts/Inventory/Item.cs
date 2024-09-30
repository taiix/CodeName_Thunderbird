using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Item", menuName ="ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public int id;

    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public GameObject itemPrefab;


    public int maxStack;
    public int inventorySize;
    public float weight;


    public enum Types
    {
        craftingMaterial,
        equipment
    }

    public enum Rarity
    {

    }

    public Types type;

}
