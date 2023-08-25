using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    public bool Paused
    {
        get
        {
            return menu.activeSelf;
        }
        set
        {
            menu.SetActive(value);
            if (value)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    void Start()
    {
        Paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) Paused = !Paused;
    }
}
