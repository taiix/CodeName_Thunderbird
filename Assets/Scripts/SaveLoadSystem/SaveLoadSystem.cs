using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveDataWrapper
{
    public List<string> jsonData;

    public SaveDataWrapper(List<string> data)
    {
        jsonData = data;
    }
}

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

        List<string> jsonData = new List<string>();

        foreach (ISavableData data in dataContainer)
        {
            jsonData.Add(data.ToJson());
        }

        SaveDataWrapper wrapper = new SaveDataWrapper(jsonData);

        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(path, json);
        Debug.Log($"Data saved successfully to {path}");
    }

    public static void LoadData(List<ISavableData> dataObjects)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveDataWrapper wrapper = JsonUtility.FromJson<SaveDataWrapper>(json);

            for (int i = 0; i < dataObjects.Count; i++)
            {
                dataObjects[i].FromJson(wrapper.jsonData[i]);
            }

            Debug.Log("Game data loaded successfully");
        }
        else
        {
            Debug.LogWarning("Save file not found");
        }
    }
}
