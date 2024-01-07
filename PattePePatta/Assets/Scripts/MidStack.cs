using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the stack of cards in the middle
/// </summary>
public class MidStack : MonoBehaviour
{
    [SerializeField] private SpriteRenderer redPlayerCard, bluePlayerCard;   // The Sprite Renderers for the top most cards of the Stack.    
    [SerializeField] private ParticleSystem RedScoreEffect, BlueScoreEffect, ScoreEffect;   // The particle effects for showing a win of the stack during the game
    
    private Stack<int> cardStack;    // Stack DS for the stack of card    
    private SpriteRenderer mainDeck;    // Sprite Renderer for the Main Deck in the middle at the begining of the game   
    private AudioSource winSound;   // Winning Sound effect
    private CardObjectPooler cardPooler;    // Object Pooling script for the card distribution and transfering animations
    private CardManager cardManager;    // For getting the card sprites from their indices
    private ScoreScript scoreScript;    // For increasing score of a player on a win
    public int turn = 0;    //  0 when its Red Player's turn, 1 when its Blue Player's turn
    public bool CanPlay = false;    // Used to make sure player is not able to play a move when card transfer animations are happening
    

    // Awake is executed at the begining before Start
    void Awake()
    {
        // Initialize and Get Reference of the required scripts and objects
        cardStack = new Stack<int>();
        cardManager = FindObjectOfType<CardManager>();
        mainDeck=GetComponent<SpriteRenderer>();
        winSound = GetComponent<AudioSource>();
        scoreScript = FindObjectOfType<ScoreScript>();
        
    }

    // Start is executed at the begining but after Awake
    private void Start()
    {
        // Get the object pooler for the card
        cardPooler=FindObjectOfType<CardObjectPooler>();
    }

    /// <summary>
    /// Run whenever there is change in the card stack so as to update the sprites of the top cards
    /// </summary>
    void UpdateDeck()
    {
        
        if (cardStack.Count == 0 || turn==-1) 
        {
            // If stack is empty, do not display the stack cards
            redPlayerCard.enabled = false;
            bluePlayerCard.enabled = false;
            return;
        }
        if (turn == 1)
        {
            // If red played move and its blue's turn, make sure the red's card is above the blue's card
            redPlayerCard.enabled=true;
            redPlayerCard.sprite = cardManager.GetCardSprite(cardStack.Peek());
        }
        else
        {
            // If blue played move and its red's turn, make sure the blue's card is above the red's card
            bluePlayerCard.enabled = true;
            bluePlayerCard.sprite = cardManager.GetCardSprite(cardStack.Peek());
        }
        
        
        
    }

    /// <summary>
    /// Run at the start of the begining when the main deck is distributed to Red and Blue players
    /// </summary>
    /// <param name="deck1">First Deck</param>
    /// <param name="deck2">Second Deck</param>
    public void DistributeCards(Deck deck1, Deck deck2)
    {
        CanPlay = false;
        mainDeck.enabled = true;
        // First shuffle and put the cards in the main deck

        int numDist = 0;    // Keep track of the number of cards  
        cardStack= new Stack<int>();    // Initialize the card stack
        bool[] cardIsDistributed = new bool[52];    // Keep track of the card so that there is no duplicate
        while (numDist < 52)
        {
            int i = Random.Range(0, 52);    // Get a random number between 0 and 51
            if (!cardIsDistributed[i])
            {
                // If this card was not pushed to the main deck before, push it
                cardStack.Push(i);
                cardIsDistributed[i] = true;    // Tick the index in the cardIsDistributed trackse
                numDist++;  // Increment numDis
            }
            
        }

        // After a Shuffled main deck is obtained, distribute the cards to the Red and Blue players
        StartCoroutine(DistributeToDecks(deck1, deck2));
    }

    /// <summary>
    /// Distribution of cards
    /// </summary>
    /// <param name="deck1">Deck 1</param>
    /// <param name="deck2">Deck 2</param>
    /// <returns></returns>
    IEnumerator DistributeToDecks(Deck deck1, Deck deck2)
    {
        // While main deck contains a card keep distributing
        while (cardStack.Count > 0)
        {           
            // Wait for a duration and distribute to Red Player
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(TransferToDeck(cardStack.Pop(),deck1,0,true));
            // Wait for a duration and distribute to Blue Player
            yield return new WaitForSeconds(0.05f);
            StartCoroutine(TransferToDeck(cardStack.Pop(),deck2,1,true));
        }

        // Hide the main deck sprite and allow player to play the move
        mainDeck.enabled = false;
        CanPlay = true;
    }

    /// <summary>
    /// To play the animation of transfering the middle stack of cards to the player who just won them
    /// </summary>
    /// <param name="deck">the deck of the winner of this stack</param>
    /// <param name="transferType">0 for transfer from red, 1 for transfer from blue</param>
    /// <returns></returns>
    IEnumerator TransferToWinner(Deck deck, int transferType)
    {
        // Prevent play of any move during this time
        CanPlay = false;

        // Wait for a second
        yield return new WaitForSeconds(1);

        // While there are cards left in the stack, keep giving them to the player
        while (cardStack.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(TransferToDeck(cardStack.Pop(), deck, transferType, false));
            UpdateDeck();
        }

        // Allow execution of moves when the transfer is over
        CanPlay = true;
    }

    /// <summary>
    /// Method for adding a card in the stack from a particular deck
    /// </summary>
    /// <param name="cardIndex">Index for getting the sprite of the card</param>
    /// <param name="deck">The deck ( red or blue) from which the card is transfered</param>
    /// <param name="transferType">0 for transfer from red, 1 for transfer from blue</param>
    public void AddToDeck(int cardIndex, Deck deck, int transferType)
    {
        StartCoroutine(TransferFromDeck(cardIndex, deck, transferType));
    }

    /// <summary>
    /// For playing the animation of transfer of a card from a deck to the mid stack
    /// </summary>
    /// <param name="cardIndex">Type of the card transfered</param>
    /// <param name="deck">Deck from which transfer is done</param>
    /// <param name="transferType">0 for transfer from red, 1 for transfer from blue</param>
    /// <returns></returns>
    IEnumerator TransferFromDeck(int cardIndex, Deck deck, int transferType)
    {
        // prevent player from executing moves
        CanPlay = false;
        float moveTime = 0.2f; // the time duration from moving the card
        GameObject card = cardPooler.GetInstance(deck.transform.position, deck.transform.rotation); // Get an instance of the card from the Object Pooler       
        SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();     // Sprite Rendered of the card instance
        cardSpriteRenderer.sortingOrder = 2;    // make sure the card is shown on top of the mid stack and the player decks
        Sprite cardSprite = cardManager.GetCardSprite(cardIndex);   // get the respective card sprite based on the index
        Sprite backSprite = transferType == 0?cardManager.redBack:cardManager.blueBack;     // get the sprite for the back side of the card  
        Vector3 startPosition = deck.transform.position;    // starting position for the animation
        Vector3 endPosition = transferType == 0 ? redPlayerCard.transform.position : bluePlayerCard.transform.position;     // ending position for the animation
        float initialAngle = (transferType - 0.5f) * 2 * 180;   // starting rotation of the card instance. Usefull to apply a flip animation.
        CardMoveScript cardMove = card.GetComponent<CardMoveScript>();  // the card move animation script of the card index
        cardMove.MoveCard(moveTime, startPosition, endPosition, new Vector3(0, initialAngle, 0), Vector3.zero, backSprite, cardSprite);     // perform the card move animation
        yield return null;  // wait for the next frame
        yield return new WaitUntil(() => !cardMove.moving); // wait until the move animation is finished
        cardPooler.RemoveInstance(card);    // remove the move card instance
        int lastCard = 0;   // get the index of the card previously on the top of the stack for comparison
        if (cardStack.Count > 0)
            lastCard = cardStack.Peek();
        
        // push the card on the mid stack
        cardStack.Push(cardIndex);

        // adjust the card on the top based on the transfer type.
        // higher sortingOrder sprites makes them to be dsplayed over the lower sortingOrder sprites
        if (transferType == 0)
        {
            bluePlayerCard.sortingOrder = 0;
            redPlayerCard.sortingOrder = 1;
        }
        else
        {
            bluePlayerCard.sortingOrder = 1;
            redPlayerCard.sortingOrder = 0;
        }

        turn = (turn + 1) % 2;  // change the turn
        UpdateDeck();   // update the display of thr top of the stack
        CanPlay = true; // allow execution of moves

        // Perform a check of the card matches with the card previously on the top
        if (cardStack.Count >=2 && cardStack.Peek() % 13 == lastCard % 13)
        {
            // If so, then Increase the score of the respective player and play the wining effects and sounds
            scoreScript.IncreaseScore(transferType);
            if (transferType == 0)
            {
                RedScoreEffect.Play();
            }
            else if (transferType == 1)
            {
                BlueScoreEffect.Play();
            }
            ScoreEffect.Play();
            winSound.Play();

            // transfer the stack to the winner
            StartCoroutine(TransferToWinner(deck, (turn + 1) % 2));
        }
    }

    /// <summary>
    /// Play the transfer to deck animation
    /// </summary>
    /// <param name="cardIndex">Index of the Card's Sprite</param>
    /// <param name="deck">Deck to which transfer is done</param>
    /// <param name="transferType">0 for transfer from red, 1 for transfer from blue</param>
    /// <param name="hidden">Whether to show the Card's fron side or hide it while transfer</param>
    /// <returns></returns>
    IEnumerator TransferToDeck(int cardIndex, Deck deck, int transferType, bool hidden = false)
    {
        float moveTime = 0.08f; // duration of animation

        GameObject card = cardPooler.GetInstance(deck.transform.position, deck.transform.rotation); // Get a card instance from the object pooler
        SpriteRenderer cardSpriteRenderer = card.GetComponent<SpriteRenderer>();    // the sprite renderer of the card
        cardSpriteRenderer.sortingOrder = 2; // make sure the card is diplayed above the decks and mid stack 
        Sprite backSprite = transferType == 0 ? cardManager.redBack : cardManager.blueBack; // backside of the card instance
        Sprite cardSprite = hidden ? backSprite : cardManager.GetCardSprite(cardIndex); // from side of the card. 
        Vector3 startPosition = transferType==0? redPlayerCard.transform.position: bluePlayerCard.transform.position; // staritng position of the animation
        Vector3 endPosition = deck.transform.position;  // ending point of the animation
        float initialAngle =(transferType - 0.5f) * 2*-180; // initial rotation for a flip effect
        Vector3 startEuler = hidden ? Vector3.zero : new Vector3(0, initialAngle, 0);   // if it is a hidden transfer, do not play the flip animation
       
        // make the card instance do the move animation
        CardMoveScript cardMove= card.GetComponent<CardMoveScript>();   
        cardMove.MoveCard(moveTime, startPosition, endPosition, startEuler, Vector3.zero, cardSprite, backSprite);
        yield return null;
        yield return new WaitUntil(() => !cardMove.moving); // wait untill move animation is finished

        // remove the card instance and add the respective card to the deck
        cardPooler.RemoveInstance(card);
        deck.AddToDeck(cardIndex);

    }

    
}
