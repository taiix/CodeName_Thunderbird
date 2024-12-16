using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float playerHealth;
    public float posX, posY, posZ;

    public PlayerData(float playerHealth, float x, float y, float z)
    {
        this.playerHealth = playerHealth;

        this.posX = x;
        this.posY = y;
        this.posZ = z;
    }
}

[Serializable]
public class PlaneData
{
    public float health;
    public float posX, posY, posZ;

    public PlaneData(float health, float x, float y, float z)
    {
        this.health = health;
        this.posX = x;
        this.posY = y;
        this.posZ = z;
    }
}

[Serializable]
public class TerrainDataSave
{
    public int seed;

    public TerrainDataSave(int seed)
    {
        this.seed = seed;
    }
}

[Serializable]
public class TimeData
{
    public string time;

    public TimeData(DateTime time)
    {
        this.time = time.ToString("HH:mm");
    }

    public DateTime GetTime()
    {
        return DateTime.Parse(time);
    }
}

[Serializable]
public class InventoryData
{
    public List<InventorySlotData> inventoryItems;

    public InventoryData(List<InventorySlotData> inventoryItems)
    {
        this.inventoryItems = inventoryItems;
    }
}

[Serializable]
public class InventorySlotData
{

    public string itemName;
    public int quantity;

    public InventorySlotData(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }

    public Item GetItem()
    {
        return Resources.Load<Item>($"Items/{itemName}");
    }
}

public class QuestData
{

}