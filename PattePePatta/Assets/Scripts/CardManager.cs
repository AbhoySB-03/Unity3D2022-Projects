using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the card sprites and provide access to them through indices.
/// The 52 cards of a deck are indexed by numers form 0 to 51
/// </summary>
public class CardManager : MonoBehaviour
{
    // List to store the card sprites
    List<Sprite> cardSprites;
    public Sprite redBack, blueBack, blackBack;
    void Awake()
    {
        // Load the sprites from the Assets
        LoadCardSprites(); 
    }

    /// <summary>
    /// Function to return a Card Sprite based on the index
    /// </summary>
    /// <param name="index">Index of the Card</param>
    /// <returns>Sprite of the card for that index</returns>
    public Sprite GetCardSprite(int index)
    {
        return cardSprites[index];
    }

    /// <summary>
    /// Function to load the sprites
    /// </summary>
    void LoadCardSprites()
    {
        // Initialize the Sprites List
        cardSprites = new List<Sprite>();

        // Sets of the Cards
        string[] cardNames = {"Club", "Diamond", "Heart", "Spade"};
        for (int i = 0; i < 52; i++) {
            string firstName = cardNames[i / 13];

            int m = (i % 13)+1;
            string lastName = m <= 9 ? "0" + m : m.ToString();

            // Obtain the sprite file name from the Index of the card
            string fileName=firstName + lastName;

            // Load the card sprite from the Assets
            cardSprites.Add(Resources.Load<Sprite>(fileName));
        }
    }
}
