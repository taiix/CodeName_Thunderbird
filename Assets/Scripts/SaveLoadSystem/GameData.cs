using System;
using System.Collections.Generic;

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

[System.Serializable]
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

[System.Serializable]
public class TerrainDataSave
{
    public int seed;

    public TerrainDataSave(int seed)
    {
        this.seed = seed;
    }
}

[System.Serializable]
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

public class InventoryData {
    public List<Item> inventoryItems;

    public InventoryData(List<Item> inventoryItems) { 
        this.inventoryItems = inventoryItems;
    }
}

public class QuestData { }