using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMenuManager : MonoBehaviour
{

    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private Animation fadeAnimation;
    [SerializeField] private float fadeDuration = 1f;

    private bool isMenuOpen = false;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void LoadScene(int sceneNum)
    {
        StartCoroutine(PlayFadeAndLoadScene(sceneNum));
    }

    private IEnumerator PlayFadeAndLoadScene(int sceneNum)
    {
        buttonsParent.SetActive(false);


        if (fadeAnimation)
        {
            fadeAnimation.Play();
        }


        yield return new WaitForSeconds(fadeDuration);


        SceneManager.LoadScene(sceneNum);
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;

        buttonsParent.SetActive(isMenuOpen);
        Cursor.visible = isMenuOpen;

        if (GameManager.Instance)
        {

            if (isMenuOpen)
            {
                GameManager.Instance.DisablePlayerControls(false);
            }
            else if (GameManager.Instance.IsPLayerInPlane())
            {
                GameManager.Instance.EnablePlayerControls(false);
            }
            else
            {
                GameManager.Instance.EnablePlayerControls(true);
            }

            if (!isMenuOpen && controlsPanel.activeSelf)
            {
                controlsPanel.SetActive(false);
                GameManager.Instance.EnablePlayerControls(true);
            }
        }
    }

    public void ToggleControlsPanel()
    {

        bool isActive = controlsPanel.activeSelf;
        controlsPanel.SetActive(!isActive);


        if (!isActive && !buttonsParent.activeSelf)
        {
            buttonsParent.SetActive(true);
            isMenuOpen = true;
        }
    }
}
