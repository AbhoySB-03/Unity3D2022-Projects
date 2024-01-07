using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains various functions for the Main Menu
/// </summary>
public class MainMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject rulesPannel;

    /// <summary>
    /// On pressing START
    /// </summary>
    public void StartGame()
    {
        // Load the Game Scene
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// On pressing GAME RULES
    /// </summary>
    public void ShowRules()
    {
        // Activate the pannel object
        rulesPannel.SetActive(true);
    }

    /// <summary>
    /// On pressing close button of GAME RULES pannel
    /// </summary>
    public void HideRules()
    {
        // Deactivate the pannel object
        rulesPannel.SetActive(false);
    }

    /// <summary>
    /// On pressing EXIT
    /// </summary>
    public void QuitApp()
    {
        Application.Quit();
    }
}
