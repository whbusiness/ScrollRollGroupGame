using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private MasterInput masterInput;
    private InputAction menu;

    [SerializeField] private GameObject pauseUI;
    private bool isPaused;
    [SerializeField]
    private NewGameController gameController;
    private void Awake()
    {
        masterInput = new MasterInput();
        menu = masterInput.Menu.Escape;
        menu.performed += TogglePause;
    }

    private void OnEnable()
    {
        menu.Enable();
    }

    private void OnDisable()
    {
        menu.Disable();
    }

    private void Start()
    {
        gameController = FindFirstObjectByType<NewGameController>();
        ResumeGame(); // Ensure the game starts in a non-paused state
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    public void LoadMainMenuAndRestart()
    {
        ResumeGame(); // Ensure the game is not paused when loading the main menu

        //Give player cusor back
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible

        //Reset NGC state
        gameController.ResetPlayers();
        Destroy(gameController.gameObject);

        // Load the main menu scene
        SceneManager.LoadScene("LVL_MENU_MAIN");

        // Restart logic specific to the main menu (you may need to modify this)
        // For example, you might want to reset some UI elements or game state
        // ...

    }
}