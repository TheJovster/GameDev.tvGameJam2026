using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame() 
    {
        ServiceRegistry.Instance?.Get<SceneLoader>()?.LoadScene("Gym");
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
