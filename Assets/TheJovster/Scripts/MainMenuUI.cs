using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private string _tutorial = "TutorialLevel";
    [SerializeField] private string _level01 = "Level01";

    public void StartGame() 
    {
        ServiceRegistry.Instance?.Get<GameManager>()?.UnpauseGame();
        ServiceRegistry.Instance?.Get<SceneLoader>()?.LoadScene(_level01);
        ServiceRegistry.Instance?.Get<AudioManager>().StopGameMusic();
        ServiceRegistry.Instance?.Get<AudioManager>()?.PlayGameMusic();
    }

    public void Tutorial() 
    {
        ServiceRegistry.Instance?.Get<AudioManager>().StopGameMusic();
        ServiceRegistry.Instance?.Get<AudioManager>()?.PlayGameMusic();
        ServiceRegistry.Instance?.Get<GameManager>()?.UnpauseGame();
        ServiceRegistry.Instance?.Get<SceneLoader>()?.LoadScene(_tutorial);
    }

    public void Options() 
    {
        //options menu is not implemented yet, so just log for now
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
