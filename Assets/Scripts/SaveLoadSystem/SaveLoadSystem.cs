using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    private static readonly string path = Path.Combine(Application.persistentDataPath, "playerData.alibaba");

    public static void SaveData(List<ISavableData> dataContainer)
    {
        if (dataContainer == null || dataContainer.Count == 0)
        {
            Debug.LogError("No data to save!");
            return;
        }

        PlayerData playerData = null;
        PlaneData planeData = null;
        TerrainDataSave terrainData = null;
        TimeData timeData = null;
        InventoryData inventoryData = null;

        List<VegetationData> vegetationData = new();

        List<TerrainDataSave> terrainSeed = new();

        foreach (ISavableData data in dataContainer)
        {
            if (data is PlayerHealth playerHealth)
            {
                playerData = new PlayerData(playerHealth.GetCurrentHealth(), playerHealth.transform.position.x, playerHealth.transform.position.y, playerHealth.transform.position.z);
            }
            else if (data is PlanePart planePart)
            {
                planeData = new PlaneData(planePart.CurrentHealth, planePart.transform.position.x, planePart.transform.position.y, planePart.transform.position.z);
            }
            else if (data is MapGeneratorGPU mapGenerator)
            {
                terrainData = new TerrainDataSave(mapGenerator.Seed);
                terrainSeed.Add(terrainData);
            }
            else if (data is TimeController timeController)
            {
                timeData = new TimeData(timeController.CurrentTime);
            }
            else if (data is InventorySystem inventorySystem)
            {
                List<InventorySlot> inventorySlots = inventorySystem.GetInventorySlots();

                List<InventorySlotData> slotsData = new List<InventorySlotData>();
                foreach (var slot in inventorySlots)
                {
                    if (slot.itemInSlot != null && slot.itemInSlot.itemName != null)
                    {
                        slotsData.Add(new InventorySlotData(slot.itemInSlot.itemName, slot.amountInSlot));
                    }
                }

                inventoryData = new InventoryData(slotsData);
            }
            else if (data is ProceduralVegetation vegetationList)
            {
                List<VegetationData> vegetationDataList = new List<VegetationData>();

                foreach (var go in vegetationList.GetSpawnedObjects())
                {
                    VegetationData d = new VegetationData(go.name, go.transform.position, go.transform.localScale);
                    vegetationDataList.Add(d);
                }


                vegetationData = vegetationDataList;
            }
        }

        GameDataContainer gameDataContainer = new GameDataContainer(playerData, planeData, terrainSeed, timeData, inventoryData, vegetationData);


        string json = JsonUtility.ToJson(gameDataContainer, true);


        File.WriteAllText(path, json);
        Debug.Log($"Data saved successfully to {path}");
    }

    public static void LoadData(List<ISavableData> dataObjects)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);

            GameDataContainer wrapper = JsonUtility.FromJson<GameDataContainer>(json);

            for (int i = 0; i < dataObjects.Count; i++)
            {
                if (dataObjects[i] is PlayerHealth playerHealth)
                {
                    string playerJson = JsonUtility.ToJson(wrapper.playerData);
                    playerHealth.FromJson(playerJson);
                }
                else if (dataObjects[i] is PlanePart planePart)
                {
                    string planeJson = JsonUtility.ToJson(wrapper.planeData);
                    planePart.FromJson(planeJson);
                }
                else if (dataObjects[i] is MapGeneratorGPU mapGenerator)
                {
                    //TerrainDataSave data = JsonUtility.FromJson<TerrainDataSave>(json);
                    //mapGenerator.FromJson(wrapper.terrainData[i]);
                }
                else if (dataObjects[i] is TimeController timeController)
                {
                    string timeJson = JsonUtility.ToJson(wrapper.timeData);
                    timeController.FromJson(timeJson);
                }
                else if (dataObjects[i] is InventorySystem inventorySystem)
                {
                    string inventoryJson = JsonUtility.ToJson(wrapper.inventoryData);
                    inventorySystem.FromJson(inventoryJson);
                }
                else if (dataObjects[i] is ProceduralVegetation vegetationList)
                {
                    //string vegetationJson = JsonUtility.ToJson(wrapper.vegetationData);
                    //vegetationList.FromJson(vegetationJson);
                }
            }

            Debug.Log("Game data loaded successfully");
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }
    }
}
