using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject _persistentSystemsPrefab;
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        if (_persistentSystemsPrefab != null)
        {
            GameObject systems = Instantiate(_persistentSystemsPrefab);
            DontDestroyOnLoad(systems);
        }

        SceneManager.LoadScene(_mainMenuSceneName);
    }
}