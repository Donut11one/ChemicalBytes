using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    //public AudioSource audioSource;

    private bool isSelected = false;

    // This is public static so that other scripts can access it. Using it to access Pause logic if needed.
    public static bool isPaused;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
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

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Pauses the game, stops animations and updates
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.visible = false;
        isSelected = false;

        // Deselect any selected UI button
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
        EventSystem.current.SetSelectedGameObject(null);

    }

    public void QuitProgram()
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
        //audioSource.Play();
    }

}
