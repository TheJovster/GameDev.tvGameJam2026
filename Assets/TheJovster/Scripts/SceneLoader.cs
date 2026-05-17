using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public event Action OnSceneLoadStarted;
    public event Action OnSceneLoadFinished;

    private bool _isLoading;

    private void Awake()
    {
        ServiceRegistry.Instance.Register<SceneLoader>(this);

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void LoadScene(int buildIndex)
    {
        if (_isLoading) return;
        StartCoroutine(LoadSceneRoutine(buildIndex));
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        LoadScene("MainMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isLoading = true;
        OnSceneLoadStarted?.Invoke();

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneRoutine(int buildIndex)
    {
        _isLoading = true;
        OnSceneLoadStarted?.Invoke();

        AsyncOperation op = SceneManager.LoadSceneAsync(buildIndex);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _isLoading = false;
        OnSceneLoadFinished?.Invoke();

        // Let ServiceRegistry re-find scene-bound references
        RefreshSceneReferences();
    }

    private void RefreshSceneReferences()
    {
        if (ServiceRegistry.Instance == null) return;

        // Camera and Player are scene-bound, re-find them after every load
        ServiceRegistry sr = ServiceRegistry.Instance;

        Camera cam = Camera.main;
        if (cam != null)
        {
            // Use reflection-free approach: ServiceRegistry exposes these directly
            // so we set them via a helper method
            sr.SetSceneReferences(cam, null);
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            sr.SetSceneReferences(null, playerObj.transform);
        }
    }
}