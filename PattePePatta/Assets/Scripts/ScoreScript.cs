using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for keeping track, updating the score and displaying it on the score board
/// </summary>
public class ScoreScript : MonoBehaviour
{
    [SerializeField] private Text redScoreText, blueScoreText;  // UI Text elements of the score board
    public int redScore, blueScore; // Keep track of the scores

    // Awake is called in the Begining of the scene
    private void Awake()
    {
        // Initialize the scores
        redScore = 0; blueScore = 0;
    }

    /// <summary>
    /// Set the value of the score for the red player
    /// </summary>
    /// <param name="score">Score for Red player</param>
    public void SetRedScore(int score)
    {
        redScore = score;
        DisplayScore();
    }

    /// <summary>
    /// Set the value of the score for the blue player
    /// </summary>
    /// <param name="score">Score for Blue Player</param>
    public void SetBlueScore(int score)
    {
        blueScore = score;
        DisplayScore();
    }

    /// <summary>
    /// Incrementing the score of the red player or blue player
    /// </summary>
    /// <param name="redorblue">0 for red, 1 for blue</param>
    public void IncreaseScore(int redorblue)
    {
        if (redorblue == 0)
        {
            redScore++;
        }
        else
        {
            blueScore++;
        }
        DisplayScore();
    }

    /// <summary>
    /// Reset the value of the scores and also the score board
    /// </summary>
    public void ResetScore()
    {
        redScore = 0;
        blueScore=0;
        DisplayScore() ;
    }

    /// <summary>
    /// Display the scores on the score board
    /// </summary>
    public void DisplayScore()
    {
        redScoreText.text=redScore.ToString();
        blueScoreText.text=blueScore.ToString();
    }

    
}
