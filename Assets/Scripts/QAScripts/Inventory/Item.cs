using UnityEngine;

[CreateAssetMenu(fileName ="Item", menuName ="ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon;
    public string itemDescription;
    public GameObject itemPrefab;


    public int maxStack;
    public int inventorySize;

    //FOR ORES ONLY
    public float miningTime;
    public int minigameDifficulty;

    public enum Types
    {
        craftingMaterial,
        miningEquipment,
        axe
    }
    public Types type;
}
