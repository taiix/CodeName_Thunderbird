using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            List<ISavableData> dataToLoad = GetAllSaveableData();

            SaveLoadSystem.LoadData(dataToLoad);
        }
    }

    public void SaveGame()
    {
        List<ISavableData> dataToSave = GetAllSaveableData();

        SaveLoadSystem.SaveData(dataToSave);
    }

    private List<ISavableData> GetAllSaveableData()
    {
        List<ISavableData> saveableData = new List<ISavableData>();

        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();

        foreach (var monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is ISavableData)
            {
                saveableData.Add(monoBehaviour as ISavableData);
            }
        }

        return saveableData;
    }
}
