using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public void LoadingLevel(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }
}
