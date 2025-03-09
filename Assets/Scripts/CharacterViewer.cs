using UnityEngine;
using TMPro;

public class CharacterViewer : MonoBehaviour
{
    [Header("Highlight Object")]
    public GameObject thumbnailHighlighter;  // The object we want to move

    [Header("Thumbnail Highlighters")]
    public GameObject boyHighlighterPosition;
    public GameObject girlHighlighterPosition;
    public GameObject newBoyHighlighterPosition;
    public GameObject newGirlHighlighterPosition;
    public GameObject EgyptQueenHighlighterPosition;
    public GameObject WitchHighlighterPosition;
    public GameObject GwenHighlighterPosition;

    [Header("Thumbnails")]
    public GameObject boyTick;
    public GameObject girlTick;
    public GameObject newBoyTick;
    public GameObject newGirlTick;
    public GameObject EgyptQueenTick;
    public GameObject GwenTick;
    public GameObject WitchTick;

    public GameStartManager gameStartManager;

    [Header("Character Models for Viewing")]
    public GameObject boyCharacterModel;
    public GameObject girlCharacterModel;
    public GameObject newBoyCharacterModel;
    public GameObject newGirlCharacterModel;
    public GameObject EgyptQueenCharacterModel;
    public GameObject WitchCharacterModel;
    public GameObject GwenCharacterModel;


    // Method to actually show/hide models
    private void ShowCharacterByKey(string characterKey)
    {
        // Disable all characters
        boyCharacterModel.SetActive(false);
        girlCharacterModel.SetActive(false);
        newBoyCharacterModel.SetActive(false);
        newGirlCharacterModel.SetActive(false);
        GwenCharacterModel.SetActive(false);
        EgyptQueenCharacterModel.SetActive(false);
        WitchCharacterModel.SetActive(false);

        // Move the highlighter to the correct position
        switch (characterKey)
        {
            case "boy":
                boyCharacterModel.SetActive(true);

                // Instantly place the highlight object here:
                thumbnailHighlighter.transform.position =
                    boyHighlighterPosition.transform.position;
                break;

            case "girl":
                girlCharacterModel.SetActive(true);

                thumbnailHighlighter.transform.position =
                    girlHighlighterPosition.transform.position;
                break;

            case "newBoy":
                newBoyCharacterModel.SetActive(true);

                thumbnailHighlighter.transform.position =
                    newBoyHighlighterPosition.transform.position;
                break;

            case "newGirl":
                newGirlCharacterModel.SetActive(true);

                thumbnailHighlighter.transform.position =
                    newGirlHighlighterPosition.transform.position;
                break;
            case "Gwen":
                GwenCharacterModel.SetActive(true);

                thumbnailHighlighter.transform.position =
                    GwenHighlighterPosition.transform.position;
                break;
            case "EgyptQueen":
                EgyptQueenCharacterModel.SetActive(true);

                thumbnailHighlighter.transform.position =
                    EgyptQueenHighlighterPosition.transform.position;
                break;
            case "Witch":
                
                WitchCharacterModel.SetActive(true);

                thumbnailHighlighter.transform.position =
                    WitchHighlighterPosition.transform.position;
                break;

            default:
                Debug.LogWarning($"Unknown character key: {characterKey}");
                break;
        }
    }

    // Called by each thumbnail button (e.g., boy, girl, newBoy, newGirl)
    // so the user can preview it visually.
    public void OnThumbnailClicked(string characterKey)
    {
        ShowCharacterByKey(characterKey);
    }

    // Called by your "Exit" button to restore the truly selected character visually
    public void ViewSelectedCharacter()
    {
        // 1) Figure out which character is truly selected in the manager
        string selectedKey = gameStartManager.GetCurrentlySelectedCharacter();

        // 2) Show that model visually
        ShowCharacterByKey(selectedKey);

        // 3) Also update GameStartManager’s "view" variables
        //    so the main action button sees the same character.
        if (gameStartManager.characterCosts.TryGetValue(selectedKey, out int cost))
        {
            gameStartManager.currentViewedCharacter = selectedKey;
            gameStartManager.currentViewedCharacterCost = cost;

            // 4) Force the main button to refresh
            gameStartManager.RefreshBigActionButton();
        }
    }
    public void RefreshThumbnailTicks()
    {
        // Boy
        bool boyOwned = gameStartManager.IsCharacterBought("boy");
        boyTick.SetActive(true);

        // Girl
        bool girlOwned = gameStartManager.IsCharacterBought("girl");
        girlTick.SetActive(true);

        // New Boy
        bool newBoyOwned = gameStartManager.IsCharacterBought("newBoy");
        newBoyTick.SetActive(newBoyOwned);

        // New Girl
        bool newGirlOwned = gameStartManager.IsCharacterBought("newGirl");
        newGirlTick.SetActive(newGirlOwned);
        
        bool GwenOwned = gameStartManager.IsCharacterBought("Gwen");
        GwenTick.SetActive(GwenOwned);

        // Egypt Queen
        bool EgyptQueenOwned = gameStartManager.IsCharacterBought("EgyptQueen");
        EgyptQueenTick.SetActive(EgyptQueenOwned);
        
        bool WitchOwned = gameStartManager.IsCharacterBought("Witch");
        WitchTick.SetActive(WitchOwned);
    }

}
