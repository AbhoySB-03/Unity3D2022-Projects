using UnityEngine;

/// <summary>
/// Usefull for playing the Card transfer animation
/// </summary>
public class CardMoveScript : MonoBehaviour
{
    private float moveTimeLeft; // keep track of the time left for the animation
    private float moveTime; // Total time for movement
    private Vector3 startPoint, endPoint;   // Start and end positions of transfer
    private Vector3 startEuler, endEuler;   // Start and end rotations of transfering. Usefull for a flip animation
    private Sprite frontSprite, backSprite; // Sprites for the Front and Back side of the Card
    private SpriteRenderer spriteRenderer;  // Srite Renderer for the cards

    private AudioSource moveSound;  // For playing the Card Transfer sound effect

    // Keep track if the card is being transfered
    public bool moving {
        get { return moveTimeLeft > 0; }
    }

    // Awake os called at the beginning of the Scene
    private void Awake()
    {
        // Get the AudioSource
        moveSound = GetComponent<AudioSource>();

        // Get the sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    

    // Update is called once per frame
    void Update()
    {
        // If animation time is left, keep updating the positions, rotations and the sprites
        if (moveTimeLeft > 0)
        {
            // Get the percentage value for the Linear Interpolation
            float perc = 1 - moveTimeLeft / moveTime;
            moveTimeLeft -= Time.deltaTime; // Reduce the Move time left by the frame time 
            transform.position = Vector3.Lerp(startPoint, endPoint, perc);  // For smooth movement from Starting Point to End Point
            transform.eulerAngles = Vector3.Lerp(startEuler, endEuler,perc);    // For a smooth flip animation
            spriteRenderer.sprite=(perc>0.55f)?backSprite:frontSprite;  // To set the Sprite Change for the flip
        }
    }

    /// <summary>
    /// Method to be called for performing the transfer animation
    /// </summary>
    /// <param name="movingTime">Time Duration of the animation</param>
    /// <param name="sourcePoint">Starting point of animation</param>
    /// <param name="destPoint">Ending point of the animation</param>
    /// <param name="startingEuler">Starting rotation in euler format</param>
    /// <param name="endingEuler">Ending rotation in euler format</param>
    /// <param name="startingSprite">The sprite to be displayed at the starting</param>
    /// <param name="endingSprite">The sprite to be displayed at the ending after the flip</param>
    public void MoveCard(float movingTime, Vector3 sourcePoint, Vector3 destPoint, Vector3 startingEuler, Vector3 endingEuler, Sprite startingSprite=null, Sprite endingSprite= null)
    {
        // The do nothing if card is already playing transfer animation
        if (moving)
            return;
        
        moveSound.Play();   // play the transfer animation sound effect
        moveTime = movingTime;  // set the total duration
        moveTimeLeft = moveTime;    // initialize the duration left parameter

        // Set Up the respective parameters
        startEuler = startingEuler; 
        endEuler = endingEuler;
        if (startingSprite != null)
            frontSprite = startingSprite;
        if (endingSprite != null)
            backSprite = endingSprite;

        startPoint = sourcePoint;
        endPoint= destPoint;

    }

}
