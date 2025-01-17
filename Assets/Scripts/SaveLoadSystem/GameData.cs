using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameDataContainer
{
    public PlayerData playerData;
    public PlaneData planeData;
    public List<TerrainDataSave> terrainData;
    public TimeData timeData;
    public InventoryData inventoryData;
    public List<VegetationData> vegetationData;

    public GameDataContainer(PlayerData playerData, PlaneData planeData, List<TerrainDataSave> terrainData, TimeData timeData, InventoryData inventoryData, List<VegetationData> vegetationData)
    {
        this.playerData = playerData;
        this.planeData = planeData;
        this.terrainData = terrainData;
        this.timeData = timeData;
        this.inventoryData = inventoryData;
        this.vegetationData = vegetationData;
    }

}

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
    public float planeDurability;
    public float posX, posY, posZ;

    public PlaneData(float health, float x, float y, float z)
    {
        this.planeDurability = health;
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

public class QuestData { }

[Serializable]
public class VegetationData
{
    public string prefabName;
    public float posX, posY, posZ;
    public float scaleX, scaleY, scaleZ;

    public VegetationData(string prefabName, Vector3 position, Vector3 scale)
    {
        this.prefabName = prefabName;
        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
        this.scaleX = scale.x;
        this.scaleY = scale.y;
        this.scaleZ = scale.z;
    }
}

[Serializable]
public class VegetationDataWrapper
{
    public List<VegetationData> data = new();
}
