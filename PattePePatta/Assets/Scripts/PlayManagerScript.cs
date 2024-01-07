using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


/// <summary>
/// Manages the overall gameplay as well as the UI functions
/// </summary>
public class PlayManagerScript : MonoBehaviour
{
    public GameObject redDeck, blueDeck; // The Red and Blue player Deck objects

    [SerializeField] private GameObject startButton, startPanel, PausePanel, GameOverPanel, RedWinsText, BlueWinsText, DrawText, helpPanel; // UI Objects
    [SerializeField] private Text GameOverScore;    // The UI Text element to show the score in the Game Over screen
    [SerializeField] private AudioMixer audioMixer; // For controlling the overall audio of the Game 
    [SerializeField] private MidStack midDeck;  //  The main deck and the mid stack object

    private Deck redPlayerDeck, bluePlayerDeck; //  The Decks Script of the Red and Blue player Decks
    private int moveTurn;   // Keep track of the whose move is next. 0 for Red's move and 1 for Blue's move
    private bool waitForCPUMove = true; // To track if waiting for the CPU's move
    private bool isPaused = false;  //  To track if the Game is paused or not

    //  Enum for the type of Gameplay, i.e, whether two player game or single player game
    public enum PlayType
    {
        vsComputer, vsPlayer
    }

    public PlayType playType = PlayType.vsComputer;     // Enum object delcaration and setting the default to single player

    // Start is called before the first frame update    
    void Start()
    {
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        startPanel.SetActive(true);
        moveTurn = 0;
        redPlayerDeck = redDeck.GetComponent<Deck>();
        bluePlayerDeck = blueDeck.GetComponent<Deck>();

    }

    // Update is called once per frame
    void Update()
    {
        // If the game is not paused and the card transfer animations are not played, allow the player to make move
        if (midDeck.CanPlay && !isPaused)
        {
            MoveMaking();
        }
    }

     /// <summary>
     /// The function to check the player inputs and apply the card moves
     /// </summary>
    void MoveMaking()
    {
        if (Input.touchCount > 0)   // If screen is touched
        {
            Touch touch = Input.GetTouch(0);    // Record the first touch
            RaycastHit hit;
            if (touch.phase == TouchPhase.Ended)    // If the user ends the touch
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 100))    // Perform a raycast to check if the touch is done above the Deck objects
                {
                    if (hit.collider.gameObject == redDeck && moveTurn == 0)    // If Red Deck is touched and it is Reds Turn to do the Move
                    {
                        CardMove(0);    // Perform the card move
                        moveTurn = 1;   // Make the next move Blue's turn
                        if (playType == PlayType.vsComputer)    // If its a single player game, then Start the CPU Move coroutine
                        {
                            StartCoroutine(CPUMove());
                        }
                    }
                    else if ((playType == PlayType.vsPlayer && hit.collider.gameObject == blueDeck && moveTurn == 1))   // Else if it is blue's turn and touch is registered above Blue deck
                    {
                        CardMove(1);    // Perform blue's card Move
                        moveTurn = 0;   // Make the next move Red's turn
                    }
                }

                // Make sure the Deck's top card is Put Down
                Vector3 pos = redDeck.transform.position;
                pos.z = 0;
                redDeck.transform.position = pos;

                pos = blueDeck.transform.position;
                pos.z = 0;
                blueDeck.transform.position = pos;


            }
            else if (touch.phase == TouchPhase.Began && Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 100))    // If user just begins to touch the screen, perform a ray cast
            {
                if (hit.collider.gameObject == redDeck && moveTurn == 0)    // If the raycast hits the Red deck ad its red's turn
                {   
                    // Lift the top card of the red deck
                    Vector3 pos = redDeck.transform.position;
                    pos.z = -1;
                    redDeck.transform.position = pos;
                }
                else if ((playType == PlayType.vsPlayer && hit.collider.gameObject == blueDeck && moveTurn == 1))   // If the raycast hits the Blue deck and its a Two player game and its Blue's turn
                {
                    // Lift the top card of the red deck
                    Vector3 pos = blueDeck.transform.position;
                    pos.z = -1;
                    blueDeck.transform.position = pos;
                }

            }
        }
        else if (playType == PlayType.vsComputer && !waitForCPUMove && moveTurn == 1 && !bluePlayerDeck.IsEmpty)    // if it is single player game and the CPU Move wait is over
        {
            CardMove(1);    // Perform Blue's card Move
            moveTurn = 0;   // Make the next move Red's tuen
            waitForCPUMove = true;  // Start waiting for CPU's next move
        }
    }

    /// <summary>
    /// Function to Distribute the card. This function will be called by the UI Distribute Button on press
    /// </summary>
    public void Distribute()
    {
        // Activate the Decks and Disable the StartDistribution button
        redDeck.SetActive(true);
        blueDeck.SetActive(true);
        startButton.SetActive(false);

        // Call the Distrivute function of the MidDeck
        midDeck.DistributeCards(redPlayerDeck, bluePlayerDeck);
    }
    
    /// <summary>
    /// Perform a card move
    /// </summary>
    /// <param name="turn">0 for Red's move, 1 for Blue's move</param>
    void CardMove(int turn)
    {
        // Perform checks if decks are empty
        if (redPlayerDeck.IsEmpty && bluePlayerDeck.DeckSize > 1)   // If Red Player's deck is Empty and Blue's deck is full enough, RED Wins
        {
            ShowResult(1);
            redDeck.SetActive(false);
            return;
        }
        else if (redPlayerDeck.DeckSize > 1 && bluePlayerDeck.IsEmpty)  // If Blue Player's deck is Empty and Red's Deck full enough, BLUE Wins
        {
            ShowResult(0);
            blueDeck.SetActive(false);
            return;
        }
        else if (redPlayerDeck.IsEmpty && bluePlayerDeck.IsEmpty)   // If both player's decks are empty, Its a DRAW
        {
            ShowResult(2);
            moveTurn = 2;
            redDeck.SetActive(false);
            blueDeck.SetActive(false);
            return;
        }


        if (turn == 0)  // If it is Red's turn, transfer a card from Red's deck to Mid Stack
        {
            midDeck.AddToDeck(redPlayerDeck.TakeFromDeck(), redPlayerDeck, 0);
        }
        else // Transfer from Blue's deck to Mid Stack
        {
            midDeck.AddToDeck(bluePlayerDeck.TakeFromDeck(), bluePlayerDeck, 1);
        }

        
    }

    /// <summary>
    /// Set up the type of Game. This functions is called by the UI Buttons for setting the Game Type
    /// </summary>
    /// <param name="type">0 for Single player Game, 1 for Two Player Game</param>
    public void SetPlayType(int type = 0)
    {
        playType = type == 1 ? PlayType.vsPlayer : PlayType.vsComputer;

        // Deactivate the Start panel and activate the Start Distribute Button
        startPanel.SetActive(false);
        startButton.SetActive(true);

    }
        
    /// <summary>
    /// Coroutine for waiting for the CPU's move
    /// </summary>
    /// <param name="time"> Duration of Wait for CPU's move. Default 0.8 secs</param>   
    IEnumerator CPUMove(float time = 0.8f)
    {
        yield return new WaitForSeconds(time);
        waitForCPUMove = false;
    }

    /// <summary>
    /// Fuction to Pause the Game. Called by the PAUSE Button in UI
    /// </summary>
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        PausePanel.SetActive(true);

    }

    /// <summary>
    /// Function to Resume the Game. Called by the RESUME Button in the Pause Menu
    /// </summary>
    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        PausePanel.SetActive(false );
    }

    /// <summary>
    /// Restart the Game. Called by the RESTART Button in the Pause Menu
    /// </summary>
    public void Restart()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Return to Main Menu. Called by Main Menu button in the Pause Menu
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// To display the Game Over screen with results
    /// </summary>
    /// <param name="type">0 for RED's win, 1 for BLUE's win, 2 for a DRAW based on Card's left</param>
    public void ShowResult(int type)
    {
        ScoreScript scoreScript = FindObjectOfType<ScoreScript>();
        GameOverPanel.SetActive(true);
        switch (type)
        {
            case 0: // When Blue is out of cards
                RedWinsText.SetActive(true);
                GameOverScore.text = "BLUE IS OUT OF CARDS\nSCORE\n  RED: " + scoreScript.redScore + "  BLUE:  " + scoreScript.blueScore; 
                break;
            case 1: // When Red is out of cards
                BlueWinsText.SetActive(true);
                GameOverScore.text = "RED IS OUT OF CARDS\nSCORE\n  RED: " + scoreScript.redScore + "  BLUE: " + scoreScript.blueScore;
                break;
            case 2: // When both are Out of Cards

                // Compare and display result base on score
                if (scoreScript.redScore > scoreScript.blueScore)
                {
                    RedWinsText.SetActive(true);
                }
                else if (scoreScript.redScore < scoreScript.blueScore)
                {
                    BlueWinsText.SetActive(true);
                }
                else
                {
                    DrawText.SetActive(true);
                }
                GameOverScore.text = "SCORE\n  RED: " + scoreScript.redScore + "\n BLUE: " + scoreScript.blueScore;
                break;
            

        }     

    }

    /// <summary>
    /// To toggle the Sound of the Game. Called by the Sound toggle in the Pause menu
    /// </summary>
    /// <param name="Value">true for Sounds, false for Silence</param>
    public void ToggleAudio(bool Value)
    {
        audioMixer.SetFloat("MusicVolume", Value ? 0 : -80);
    }

    /// <summary>
    /// On pressing HELP button
    /// </summary>
    public void ShowHelp()
    {
        // Activate the pannel object
        helpPanel.SetActive(true);
    }

    /// <summary>
    /// On pressing close button of HELP pannel
    /// </summary>
    public void HideHelp()
    {
        // Deactivate the pannel object
        helpPanel.SetActive(false);
    }
}
