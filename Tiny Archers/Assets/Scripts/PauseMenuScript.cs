using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private AudioMixer audioMixer;
    public bool isPaused;
    

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
   

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void SetAudio(bool value)
    {
        audioMixer.SetFloat("volume", value ? 0 : -80);
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
