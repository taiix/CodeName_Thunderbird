using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;
    [SerializeField] private Slider loadingBar;

    private void Awake()
    {
        // Make sure there's only one instance of the manager
        if (instance != null)
        {
            Destroy(gameObject);  // Destroy this duplicate
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Prevent this object from being destroyed between scenes
        }
    }

    public void LoadScenelevel(int index)
    {
        SceneManager.LoadScene(index);
        Destroy(gameObject);
    }

    public void LoadSavedGame(int index)
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    public void QuitGame()
    {
        Application.Quit();
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

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        loadingBar.transform.parent.gameObject.SetActive(true);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingBar.value = progress;
            yield return null;
        }

        yield return new WaitForSeconds(.1f);

        List<ISavableData> dataToLoad = GetAllSaveableData();

        if (dataToLoad.Count > 0)
        {
            SaveLoadSystem.LoadData(dataToLoad);
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("No saveable data found to load.");
        }
    }
}

