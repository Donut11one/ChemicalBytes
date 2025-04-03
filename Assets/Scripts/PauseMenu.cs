using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    // This is public static so that other scripts can access it. Using it to access Pause logic if needed.
    public static bool isPaused;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    /// <summary>
    /// Called once per frame, checking for input to pause the game.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                PauseGame();
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

            }
        }

    }

    /// <summary>
    /// Pauses the game and displays the pause menu, freezing all activity in game.
    /// </summary>
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Pauses the game, stops animations and updates
        isPaused = true;
    }

    /// <summary>
    /// Event handler for the "Resume" button.
    /// </summary>
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;

        // Deselect any selected UI button
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Event handler for the "Main Menu" button.
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
        EventSystem.current.SetSelectedGameObject(null);

    }

    /// <summary>
    /// Event handler for the "Quit" button.
    /// </summary>
    public void QuitProgram()
    {
        Application.Quit();
    }

    /// <summary>
    /// Simple helper to attach audio resource to the button.
    /// </summary>
    public void Selected()
    {
    }

    /// <summary>
    /// Simple helper to attach audio resource to the button.
    /// </summary>
    public void Deselected()
    {
    }

    /// <summary>
    /// Simple helper to attach audio resource to the button.
    /// </summary>
    public void HoverEnter()
    {
    }

}
