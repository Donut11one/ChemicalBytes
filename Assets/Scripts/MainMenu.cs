using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartCreative()
    {
        SceneManager.LoadSceneAsync(1);
    }
    
    public void StartQuiz()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void Selected()
    {

    }

    public void Deselected()
    {

    }

    public void HoverEnter()
    {

    }
}
