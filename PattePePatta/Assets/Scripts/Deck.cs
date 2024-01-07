using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For managing and displaying a deck of card
/// </summary>
public class Deck : MonoBehaviour
{
    [SerializeField] private GameObject cardsBelow; // For making the deck look thick when there are more than 1 cards present
    [SerializeField] private Text cardleftText; // For displaying the number of cards present in the deck in the UI

    private SpriteRenderer spriteRenderer; // Sprite renderer for the deck
    private Stack<int> cardStack;   // The stack DS for the cards in the deck
    
    void Awake()
    {
        // initialize variavles and get references 
        cardStack = new Stack<int>();
        spriteRenderer=GetComponent<SpriteRenderer>();

        // Update the look of the deck
        UpdateDeck();
    }

    /// <summary>
    /// Method to update the look of the deck
    /// </summary>
    void UpdateDeck()
    {
        // Do not display the deck if it is empty
        spriteRenderer.enabled = cardStack.Count>0;

        // Display only one card if there is one card present
        cardsBelow.SetActive(cardStack.Count>1);

        // Display other cards if there are more than one card present
        cardleftText.text=cardStack.Count.ToString();
    }

    /// <summary>
    /// Add a card to the deck
    /// </summary>
    /// <param name="cardIndex">Index of the card</param>
    public void AddToDeck(int cardIndex)
    {
        // Push the card index to the stack and update the look of the deck
        cardStack.Push(cardIndex);
        UpdateDeck();
    }

    /// <summary>
    /// Remove the card from the deck
    /// </summary>
    public int TakeFromDeck()
    {
        // Pop the card index from the stack and update the look. 
        int cardIndex = cardStack.Pop();
        UpdateDeck();

        // Also return the card index
        return cardIndex;
    }

    /// <summary>
    /// If the deck is filled or empty
    /// </summary>
    public bool IsEmpty { get { return cardStack.Count == 0; } }

    /// <summary>
    /// Number of cards in the deck
    /// </summary>
    public int DeckSize
    {
        get { return cardStack.Count; }
    }
    
}
