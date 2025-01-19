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
        TimeData timeData = null;
        InventoryData inventoryData = null;

        AllIslandsVegetation allIslandsVegetation = new();
        List<TerrainDataSave> terrainSeed = new();

        foreach (ISavableData data in dataContainer)
        {
            try
            {
                switch (data)
                {
                    case PlayerHealth playerHealth:
                        playerData = JsonUtility.FromJson<PlayerData>(data.ToJson());
                        break;

                    case PlanePart planePart:
                        planeData = JsonUtility.FromJson<PlaneData>(data.ToJson());
                        break;

                    case MapGeneratorGPU mapGenerator:
                        terrainSeed.Add(JsonUtility.FromJson<TerrainDataSave>(data.ToJson()));
                        break;

                    case TimeController timeController:
                        timeData = JsonUtility.FromJson<TimeData>(data.ToJson());
                        break;

                    case InventorySystem inventorySystem:
                        inventoryData = JsonUtility.FromJson<InventoryData>(data.ToJson());
                        break;

                    case ProceduralVegetation vegetationList:
                        {
                            List<VegetationData> vegetationDataList = new List<VegetationData>();

                            var spawns = vegetationList.GetSpawnedObjects();
                            Debug.Log($"[Save] {vegetationList.gameObject.name} has {spawns.Count} objects.");

                            foreach (var go in spawns)
                            {
                                if (go == null || string.IsNullOrEmpty(go.name))
                                {
                                    Debug.LogWarning($"Skipped saving null or unnamed GameObject in {vegetationList.gameObject.name}.");
                                    continue;
                                }

                                VegetationData d = new VegetationData(go.name, go.transform.position, go.transform.localScale);
                                vegetationDataList.Add(d);
                            }

                            if (vegetationDataList.Count > 0)
                            {
                                VegetationDataWrapper islandWrapper = new VegetationDataWrapper(vegetationDataList);
                                allIslandsVegetation.islandsData.Add(islandWrapper);
                            }
                            else
                            {
                                Debug.LogWarning($"No vegetation objects to save for {vegetationList.gameObject.name}.");
                            }
                        }
                        break;

                    default:
                        Debug.LogWarning($"Unhandled data type: {data.GetType().Name}");
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error saving data for {data.GetType().Name}: {ex.Message}");
            }
        }
        GameDataContainer gameDataContainer = new GameDataContainer(playerData, planeData, terrainSeed,
            timeData, inventoryData, allIslandsVegetation);

        string json = JsonUtility.ToJson(gameDataContainer, true);


        File.WriteAllText(path, json);
        Debug.Log($"Data saved successfully to {path}");
    }

    public static void LoadData(List<ISavableData> dataObjects)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("Save file not found");
            return;
        }

        try
        {
            // Read JSON from file
            string json = File.ReadAllText(path);
            GameDataContainer wrapper = JsonUtility.FromJson<GameDataContainer>(json);

            foreach (ISavableData data in dataObjects)
            {
                try
                {
                    switch (data)
                    {
                        case PlayerHealth playerHealth:
                            if (wrapper.playerData != null)
                            {
                                playerHealth.FromJson(JsonUtility.ToJson(wrapper.playerData));
                            }
                            break;

                        case PlanePart planePart:
                            if (wrapper.planeData != null)
                            {
                                planePart.FromJson(JsonUtility.ToJson(wrapper.planeData));
                            }
                            break;

                        case MapGeneratorGPU mapGenerator:
                            if (wrapper.terrainData.Count > 0)
                            {
                                string terrainJson = JsonUtility.ToJson(wrapper.terrainData[0]);
                                mapGenerator.FromJson(terrainJson);
                                wrapper.terrainData.RemoveAt(0); // Remove used terrain data
                            }
                            break;

                        case TimeController timeController:
                            if (wrapper.timeData != null)
                            {
                                timeController.FromJson(JsonUtility.ToJson(wrapper.timeData));
                            }
                            break;

                        case InventorySystem inventorySystem:
                            if (wrapper.inventoryData != null)
                            {
                                inventorySystem.FromJson(JsonUtility.ToJson(wrapper.inventoryData));
                            }
                            break;

                        case ProceduralVegetation vegetationList:
                            if (wrapper.vegetationAllIslands.islandsData.Count > 0)
                            {
                                string vegetationJson = JsonUtility.ToJson(wrapper.vegetationAllIslands.islandsData[0]);
                                vegetationList.FromJson(vegetationJson);
                                wrapper.vegetationAllIslands.islandsData.RemoveAt(0); // Remove used vegetation data
                            }
                            break;

                        default:
                            Debug.LogWarning($"Unhandled data type: {data.GetType().Name}");
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error loading data for {data.GetType().Name}: {ex.Message}");
                }
            }

            Debug.Log("Game data loaded successfully");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load game data: {ex.Message}");
        }
    }
}
