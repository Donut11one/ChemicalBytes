using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartCreative()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
