using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private bool isSelected = false;
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
        isSelected = true;
    }

    public void Deselected()
    {
        isSelected = false;
    }

    public void HoverEnter()
    {

    }
}
