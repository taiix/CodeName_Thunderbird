using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            List<ISavableData> dataToSave = GetAllSaveableData();

            SaveLoadSystem.SaveData(dataToSave );
        } 
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            List<ISavableData> dataToLoad = GetAllSaveableData();

            SaveLoadSystem.LoadData(dataToLoad);
        }
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
