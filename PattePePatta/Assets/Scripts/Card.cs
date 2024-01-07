using UnityEngine;

/// <summary>
/// For managing the look of a card object
/// </summary>
public class Card : MonoBehaviour
{   
    private int cardIndex;  // index to identify the card
    private SpriteRenderer spriteRenderer;  // sprite renderer of the card
    private CardManager cardManager;    // for the cardManager script to obtain the desired sprite

    // Awake is called in the beggining
    void Awake()
    {
        // Initialize variables and Get dersired references
        cardIndex = 0;  
        cardManager=FindObjectOfType<CardManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    /// <summary>
    /// Set the Look of the card from the card index passed to it
    /// </summary>
    /// <param name="index">The index that denote a particular card from the deck</param>
    public void SetCardIndex(int index)
    {
        cardIndex = index;
        spriteRenderer.sprite=cardManager.GetCardSprite(cardIndex);
    }

    /// <summary>
    /// Get the card index of this card
    /// </summary>
    /// <returns>Index of the card</returns>
    public int GetCardIndex()
    {
        return cardIndex;
    }


    /// <summary>
    /// Make this card display on top of other cards
    /// </summary>
    public void BringOnTop()
    {
        spriteRenderer.sortingOrder = 1;
    }

    /// <summary>
    /// Make this card display at the bottom of the other cards
    /// </summary>
    public void SetToBottom()
    {
        spriteRenderer.sortingOrder=0;
    }
}
