using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameStartManager : MonoBehaviour
{

    [Header("Character Display Name")]
    public TextMeshProUGUI selectedCharacterNameText; // Assign in inspector
    public Dictionary<string, string> characterDisplayNames = new Dictionary<string, string>()
{
    { "boy", "Zen" },
    { "girl", "Luna" },
    { "newBoy", "NewZen" },
    { "newGirl", "NewLuna" },
    { "Gwen", "Gwen" },
    { "Witch", "Drucilia" },
    { "EgyptQueen", "Amelia" }
};

    public CharacterViewer characterViewer;
    [Header("Costs for Characters")]
    public int newBoyCost = 1000;
    public int newGirlCost = 2000;
    public int GwenCost = 3000;
    public int WitchCost = 3000;
    public int EgyptQueenCost = 3000;

    [Header("UI Elements")]
    public GameObject homePageUI;
    public GameObject inGameUI;
    public Button startButton;

    [Header("Character Buttons")]
    public Button boyButton;
    public Button girlButton;
    public Button newBoyButton;
    public Button newGirlButton;
    public Button GwenButton;
    public Button WitchButton;
    public Button EgyptQueenButton;



    [Header("Menu Characters (Start Menu)")]
    public GameObject boyCharacterMenu;
    public GameObject girlCharacterMenu;
    public GameObject newBoyCharacterMenu;
    public GameObject newGirlCharacterMenu;
    public GameObject GwenCharacterMenu;
    public GameObject WitchCharacterMenu;
    public GameObject EgyptQueenCharacterMenu;

    [Header("In-Game Characters")]
    public GameObject boyCharacterGame;
    public GameObject girlCharacterGame;
    public GameObject newBoyCharacterGame;
    public GameObject newGirlCharacterGame;
    public GameObject GwenCharacterGame;
    public GameObject WitchCharacterGame;
    public GameObject EgyptQueenCharacterGame;

    [Header("Tutorial Characters")]
    public GameObject tutorialBoyCharacter;
    public GameObject tutorialGirlCharacter;
    public GameObject tutorialNewBoyCharacter;
    public GameObject tutorialNewGirlCharacter;
    public GameObject tutorialGwenCharacter;
    public GameObject tutorialWitchCharacter;
    public GameObject tutorialEgyptQueenCharacter;

    [Header("Character Position Targets")]
    public GameObject boyPositionTarget;
    public GameObject girlPositionTarget;
    public GameObject newBoyPositionTarget;
    public GameObject newGirlPositionTarget;
    public GameObject GwenPositionTarget;
    public GameObject WitchPositionTarget;
    public GameObject EgyptQueenPositionTarget;

    [Header("Camera and Follow Scripts")]
    public CameraFollow cameraFollow; // Main game camera
    public CameraFollow mainCameraFollow; // Reference to the main game camera
    public CameraFollow tutorialCameraFollow; // Reference to the tutorial camera

    [Header("Animation Controllers")]
    public Animator startButtonAnimator;
    public Animator GlossaryAnimator;
    public Animator Character_Selector_Animator;
    public Animator TItle_Animator;
    public Animator Settings_Animator;
    public Animator Highscore_Animator;

    [Header("Other References")]
    public GameObject selectSphere; // Reference to the SelectSphere GameObject
    public GameObject tutorialGameObject; // Reference to the tutorial game object
    public GroundSpawnerTest groundSpawner; // Ground spawner reference
    private GameObject selectedCharacter;
    private Transform selectedCharacterTransform;
    public Canvas CharSelectCanvas;

    public int userGems = 9000;
/*    public  TextMeshProUGUI gemText;
    public  TextMeshProUGUI gemText_InChar_View;*/

    // The single big button and its text
    public GameObject Diamond;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;

    // ADDED: these track which character is currently "viewed" for the single button logic
    public string currentViewedCharacter = "";
    public int currentViewedCharacterCost = 0;



    void Start()
    {
        string initialChar = PlayerPrefs.GetString("SelectedCharacter", "boy");
        if (characterDisplayNames.TryGetValue(initialChar, out string initialName))
        {
            selectedCharacterNameText.text = initialName;
        }

        if (actionButton != null)

            actionButton.onClick.AddListener(OnActionButtonClicked);

        // 1) (Optional) Load user coins from PlayerPrefs if you're persisting them
        // userCoins = PlayerPrefs.GetInt("UserCoins", 5000);

        // 2) Update coin text if you have a coinText
        userGems = PlayerPrefs.GetInt("UserGems", 5000);
        UiManager.Instance.DiamondUpdate(userGems);


        // 3) Start button logic
        if (startButton != null)
        {
            if (PlayerPrefs.GetInt("FirstTimeOpen", 1) == 1)
            {
                startButton.onClick.AddListener(StartTutorial);
            }
            else
            {
                startButton.onClick.AddListener(StartGame);
            }
        }

        // 4) Set up newGirl & newBoy purchase logic
        if (newGirlButton != null)
        {
            newGirlButton.onClick.AddListener(() => OnThumbnailClicked("newGirl"));
        }
        if (newBoyButton != null)
        {
            newBoyButton.onClick.AddListener(() => OnThumbnailClicked("newBoy"));
        }
        if (boyButton != null)
        {
            boyButton.onClick.AddListener(() => OnThumbnailClicked("boy"));
        }
        if (girlButton != null)
        {
            girlButton.onClick.AddListener(() => OnThumbnailClicked("girl"));
        } 
        if (GwenButton != null)
        {
            GwenButton.onClick.AddListener(() => OnThumbnailClicked("Gwen"));
        }
        if (WitchButton != null)
        {
            WitchButton.onClick.AddListener(() => OnThumbnailClicked("Witch"));
        }
        if (EgyptQueenButton != null)
        {
            EgyptQueenButton.onClick.AddListener(() => OnThumbnailClicked("EgyptQueen"));
        }

        // 6) Hide/show the correct UI screens at the start
        homePageUI.SetActive(true);
        inGameUI.SetActive(false);
        tutorialGameObject.SetActive(false);

        // 7) Hide tutorial chars if not needed
        tutorialBoyCharacter.SetActive(false);
        tutorialGirlCharacter.SetActive(false);

        // 8) Load whichever character was saved
        LoadSelectedCharacter();

        // 9) Refresh button states
        RefreshAllCharacterButtons();
       // characterViewer.RefreshThumbnailTicks();
    }

    /* public static UpdateGems(int diamondCount)
     {
         gemText_InChar_View.text = diamondCount.ToString();
     }*/
    public void ResetAllPurchasesAndGems()
    {
        // reset user gems in PlayerPrefs
        PlayerPrefs.DeleteKey("UserGems");

        // reset purchased flags
        PlayerPrefs.DeleteKey("newBoyBought");
        PlayerPrefs.DeleteKey("newGirlBought");
        PlayerPrefs.DeleteKey("WitchBought");
        PlayerPrefs.DeleteKey("GwenBought");
        PlayerPrefs.DeleteKey("EgyptQueenBought");
        // PlayerPrefs.DeleteKey("boyBought"); 
        // PlayerPrefs.DeleteKey("girlBought"); if needed

        // reset selected char
        PlayerPrefs.SetString("SelectedCharacter", "boy");
        PlayerPrefs.Save();
        userGems = 5000; // or 0, whatever your default is
    

        Debug.Log("All purchases and gems have been reset!");
        RefreshAllCharacterButtons();
    }

    // ADDED: A simple function if you have small thumbnail buttons
    // for each character. They call this with their own key + cost.
    public void OnThumbnailClicked(string charKey)
    {
        // Look up the cost in the dictionary
        if (characterCosts.TryGetValue(charKey, out int cost))
        {
            currentViewedCharacter = charKey;
            currentViewedCharacterCost = cost;
            RefreshBigActionButton();
        }
        else
        {
            Debug.LogError($"[OnThumbnailClicked] No entry in dictionary for key '{charKey}'");
        }
        if (selectedCharacterNameText != null)
        {
            if (characterDisplayNames.TryGetValue(charKey, out string displayName))
            {
                selectedCharacterNameText.text = displayName;
            }
            else
            {
                selectedCharacterNameText.text = "Adventurer";
            }
        }
    }

    [Header("Character Costs")]

    public Dictionary<string, int> characterCosts = new Dictionary<string, int>
    {
    { "boy", 0 },
    { "girl", 0 },
    { "newBoy", 1000 },
    { "newGirl", 1000 },
    { "Gwen", 1000 },
    { "Witch", 1000 },
    { "EgyptQueen", 1000 }
    };

    public string GetCurrentlySelectedCharacter()
    {
        // Return whichever is stored, default to "boy" if none
        return PlayerPrefs.GetString("SelectedCharacter", "boy");
    }


    public void RefreshBigActionButton()
    {
        // If no character is viewed, disable or hide the button
        if (string.IsNullOrEmpty(currentViewedCharacter))
        {
            actionButtonText.text = "";
            actionButton.interactable = false;
            return;
        }

        // Check if purchased
        bool isPurchased = IsCharacterBought(currentViewedCharacter)
                           || currentViewedCharacterCost == 0;

        // Who's currently selected?
        string currentSelected = PlayerPrefs.GetString("SelectedCharacter", "boy");

        if (!isPurchased)
        {
            // Not purchased => show cost
            actionButtonText.text = $"{currentViewedCharacterCost}";
            actionButton.interactable = true;
            Diamond.gameObject.SetActive(true);
        }
        else
        {
            // If purchased
            if (currentSelected == currentViewedCharacter)
            {
                // Already selected
                actionButtonText.text = "Selected";
                actionButton.interactable = false;
            }
            else
            {
                // Owned but not currently selected
                actionButtonText.text = "Select";
                actionButton.interactable = true;
            }
            Diamond.gameObject.SetActive(false);
        }
    }

    private void OnActionButtonClicked()
    {
        // If no character is viewed, do nothing
        if (string.IsNullOrEmpty(currentViewedCharacter)) return;

        string buttonLabel = actionButtonText.text;

        if (buttonLabel.Contains($"{currentViewedCharacterCost}"))
        {
            // The user wants to buy
            AttemptToBuy(currentViewedCharacter, currentViewedCharacterCost);
        }
        else if (buttonLabel == "Select")
        {
            // The user wants to select
            SelectCharacter(currentViewedCharacter);
            // Optionally refresh
            RefreshBigActionButton();
        }
        else if (buttonLabel == "Selected")
        {
            Debug.Log("Character is already selected!");
        }
    }
    private void AttemptToBuy(string charKey, int cost)
    {
        if (userGems >= cost)
        {
            userGems -= cost;  // subtract the cost

            // Save new gem total to PlayerPrefs
            PlayerPrefs.SetInt("UserGems", userGems);
            PlayerPrefs.Save();
            UiManager.Instance.DiamondUpdate(userGems);
           

            // Mark the char as purchased
            PlayerPrefs.SetInt($"{charKey}Bought", 1);
            PlayerPrefs.Save();

            // Optionally select it right away
            SelectCharacter(charKey);

            RefreshBigActionButton();
            characterViewer.RefreshThumbnailTicks();
        }
        else
        {
            Debug.Log("Not enough gems to buy " + charKey);
        }
    }
/*
    private void OnNewBoyButtonClicked()
    {
        // If not purchased yet, either show buy UI or buy directly:
        if (!IsCharacterBought("newBoy"))
        {
            // Show purchase logic or a confirmation popup:
            // E.g. if user has enough coins:
            if (userGems >= newBoyCost)
            {
                // Deduct cost
                userGems -= newBoyCost;
                PlayerPrefs.SetInt("UserGems", userGems);

                // Mark as purchased
                PlayerPrefs.SetInt("newBoyBought", 1);
                PlayerPrefs.Save();

                // Now that it's purchased, let them select
                SelectCharacter(newBoyCharacterMenu, newBoyCharacterGame.transform, "newBoy", newBoyPositionTarget.transform.position);
            }
            else
            {
                Debug.Log("Not enough coins to buy newBoy!");
                // Or open a "not enough coins" message
            }
        }
        else
        {
            // Already purchased => just select
            SelectCharacter(newBoyCharacterMenu, newBoyCharacterGame.transform, "newBoy", newBoyPositionTarget.transform.position);
        }

        RefreshAllCharacterButtons();
    }*/

/*
    private void OnNewGirlButtonClicked()
    {
        // Check if "newGirl" is already purchased
        if (!IsCharacterBought("newGirl"))
        {
            // Not purchased yet => attempt to buy
            if (userGems >= newGirlCost)
            {
                // Deduct coins
                userGems -= newGirlCost;
                PlayerPrefs.SetInt("UserGems", 5000);

                // Mark as purchased => MUST set to 1
                PlayerPrefs.SetInt("newGirlBought", 1);
                PlayerPrefs.Save();

                // Now that it's purchased, we can select it
                SelectCharacter(newGirlCharacterMenu, newGirlCharacterGame.transform,
                                "newGirl", newGirlPositionTarget.transform.position);
            }
            else
            {
                Debug.Log("Not enough coins to buy newGirl!");
                // Optional: Show a popup or UI indicating insufficient coins
            }
        }
        else
        {
            // Already purchased => just select
            SelectCharacter(newGirlCharacterMenu, newGirlCharacterGame.transform,
                            "newGirl", newGirlPositionTarget.transform.position);
        }

        // Refresh UI states
        RefreshAllCharacterButtons();
    }
*/
    public bool IsCharacterBought(string charKey)
    {
        // We'll match our PlayerPrefs usage:
        // "newBoy" => "newBoyBought" 
        // "newGirl" => "newGirlBought"
        // "boy" => free? or "boyBought" if you want to track that
        // "girl" => free? or "girlBought"
        // Return true if stored int = 1
        return PlayerPrefs.GetInt($"{charKey}Bought", 0) == 1;
    }

    public void StartTutorial()
    {
        if (selectedCharacter == null || selectedCharacterTransform == null)
        {
            Debug.LogError("No character selected!");
            return;
        }

        if (groundSpawner != null)
        {
            groundSpawner.isTutorialMode = true;
            Debug.Log("Tutorial mode enabled!");
        }

        if (startButtonAnimator != null)
        {
            startButtonAnimator.SetTrigger("Start");
        }

        PlayerPrefs.SetInt("FirstTimeOpen", 0);
        PlayerPrefs.Save();

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartGame);

        StartCoroutine(TransitionToTutorialUI());
        LoadSelectedCharacter();
    }

    public void EndTutorial()
    {
        if (groundSpawner != null)
        {
            groundSpawner.isTutorialMode = false;
            Debug.Log("Tutorial mode disabled!");
        }

        StartGame();
    }

    private IEnumerator TransitionToTutorialUI()
    {
        yield return new WaitForSeconds(3.5f);

        homePageUI.SetActive(false);
        tutorialGameObject.SetActive(true);

        tutorialBoyCharacter.SetActive(false);
        tutorialGirlCharacter.SetActive(false);
        tutorialNewGirlCharacter.SetActive(false);
        tutorialNewBoyCharacter.SetActive(false);
        tutorialGwenCharacter.SetActive(false);
        tutorialEgyptQueenCharacter.SetActive(false);
        tutorialWitchCharacter.SetActive(false);

        if (selectedCharacterTransform == boyCharacterGame.transform)
        {
            tutorialBoyCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialBoyCharacter.transform;
        }
        else if (selectedCharacterTransform == girlCharacterGame.transform)
        {
            tutorialGirlCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialGirlCharacter.transform;
        }
        else if (selectedCharacterTransform == newGirlCharacterGame.transform)
        {
            tutorialNewGirlCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialNewGirlCharacter.transform;
        }
        else if (selectedCharacterTransform == newBoyCharacterGame.transform)
        {
            tutorialNewBoyCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialNewBoyCharacter.transform;
        }
        else if (selectedCharacterTransform == GwenCharacterGame.transform)
        {
            tutorialGwenCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialGwenCharacter.transform;
        }
        else if (selectedCharacterTransform == EgyptQueenCharacterGame.transform)
        {
            tutorialEgyptQueenCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialEgyptQueenCharacter.transform;
        }
        
        else if (selectedCharacterTransform == WitchCharacterGame.transform)
        {
            tutorialWitchCharacter.SetActive(true);
            tutorialCameraFollow.target = tutorialWitchCharacter.transform;
        }

        mainCameraFollow.enabled = false;
        tutorialCameraFollow.enabled = true;
    }

    public void StartGame()
    {
        if (selectedCharacter == null || selectedCharacterTransform == null)
        {
            Debug.LogError("No character selected!");
            return;
        }

        if (startButtonAnimator != null)
        {
            startButtonAnimator.SetTrigger("Start");
        }

        StartCoroutine(TransitionToInGameUI());
        ResetAssets();
    }

    private IEnumerator TransitionToInGameUI()
    {
        yield return new WaitForSeconds(3.5f);

        homePageUI.SetActive(false);
        inGameUI.SetActive(true);

        DeactivateUnselectedCharacters();
    }

    private void DeactivateUnselectedCharacters()
    {
        if (selectedCharacterTransform == boyCharacterGame.transform)
        {
            girlCharacterGame?.SetActive(false);
            newGirlCharacterGame?.SetActive(false);
            newBoyCharacterGame?.SetActive(false);
            GwenCharacterGame?.SetActive(false);
            EgyptQueenCharacterGame?.SetActive(false);
            WitchCharacterGame?.SetActive(false);
        }
        else if (selectedCharacterTransform == girlCharacterGame.transform)
        {
            boyCharacterGame?.SetActive(false);
            newGirlCharacterGame?.SetActive(false);
            newBoyCharacterGame?.SetActive(false);
            GwenCharacterGame?.SetActive(false);
            EgyptQueenCharacterGame?.SetActive(false);
            WitchCharacterGame?.SetActive(false);
        }
        else if (selectedCharacterTransform == newGirlCharacterGame.transform)
        {
            boyCharacterGame?.SetActive(false);
            girlCharacterGame?.SetActive(false);
            newBoyCharacterGame?.SetActive(false);
            GwenCharacterGame?.SetActive(false);
            EgyptQueenCharacterGame?.SetActive(false);
            WitchCharacterGame?.SetActive(false);
        }
        else if (selectedCharacterTransform == newBoyCharacterGame.transform)
        {
            boyCharacterGame?.SetActive(false);
            girlCharacterGame?.SetActive(false);
            newGirlCharacterGame?.SetActive(false);
            GwenCharacterGame?.SetActive(false);
            EgyptQueenCharacterGame?.SetActive(false);
            WitchCharacterGame?.SetActive(false);
        }
        else if (selectedCharacterTransform == GwenCharacterGame.transform)
        {
            boyCharacterGame?.SetActive(false);
            girlCharacterGame?.SetActive(false);
            newGirlCharacterGame?.SetActive(false);
            newBoyCharacterGame?.SetActive(false);
            EgyptQueenCharacterGame?.SetActive(false);
            WitchCharacterGame?.SetActive(false);
        }
        else if (selectedCharacterTransform == EgyptQueenCharacterGame.transform)
        {
            boyCharacterGame?.SetActive(false);
            girlCharacterGame?.SetActive(false);
            newGirlCharacterGame?.SetActive(false);
            newBoyCharacterGame?.SetActive(false);
            WitchCharacterGame?.SetActive(false);
            GwenCharacterGame?.SetActive(false);
        }
        else if (selectedCharacterTransform == WitchCharacterGame.transform)
        {
            boyCharacterGame?.SetActive(false);
            girlCharacterGame?.SetActive(false);
            newGirlCharacterGame?.SetActive(false);
            newBoyCharacterGame?.SetActive(false);
            EgyptQueenCharacterGame?.SetActive(false);
            GwenCharacterGame?.SetActive(false);
        }
    }

    private void SelectCharacter(string charKey)
    {
        switch (charKey)
        {
            case "boy":
                SelectCharacter(boyCharacterMenu, boyCharacterGame.transform, "boy", boyPositionTarget.transform.position);
                
                break;

            case "girl":
                SelectCharacter(girlCharacterMenu, girlCharacterGame.transform, "girl", girlPositionTarget.transform.position);
                break;

            case "newBoy":
                SelectCharacter(newBoyCharacterMenu, newBoyCharacterGame.transform, "newBoy", newBoyPositionTarget.transform.position);
                break;

            case "newGirl":
                SelectCharacter(newGirlCharacterMenu, newGirlCharacterGame.transform, "newGirl", newGirlPositionTarget.transform.position);
                break;

            case "Gwen":
                SelectCharacter(GwenCharacterMenu, GwenCharacterGame.transform, "Gwen", GwenPositionTarget.transform.position);
                break;
            case "EgyptQueen":
                SelectCharacter(EgyptQueenCharacterMenu, EgyptQueenCharacterGame.transform, "EgyptQueen", EgyptQueenPositionTarget.transform.position);
                break;
            case "Witch":
                SelectCharacter(WitchCharacterMenu, WitchCharacterGame.transform, "Witch", WitchPositionTarget.transform.position);
                break;
            
            default:
                Debug.LogWarning("Unknown charKey: " + charKey);
                break;
        }
    }

    private void SelectCharacter(
    GameObject characterMenu,
    Transform characterTransform,
    string characterType,
    Vector3 spherePosition)
    {
        // If the character is not bought, we do NOT want to allow selection.
        // But you already handle the buy logic in the button click above.
        if (!IsCharacterBought(characterType) && (characterType == "newBoy" || characterType == "newGirl"))
        {
            Debug.Log("This character is not purchased yet. Cannot select!");
            return;
        }

        selectedCharacter = characterMenu;
        selectedCharacterTransform = characterTransform;

        // Enable/disable the correct character menu objects, e.g.:
        boyCharacterMenu.SetActive(characterMenu == boyCharacterMenu);
        girlCharacterMenu.SetActive(characterMenu == girlCharacterMenu);
        newBoyCharacterMenu.SetActive(characterMenu == newBoyCharacterMenu);
        newGirlCharacterMenu.SetActive(characterMenu == newGirlCharacterMenu);
        GwenCharacterMenu.SetActive(characterMenu == GwenCharacterMenu);
        EgyptQueenCharacterMenu.SetActive(characterMenu == EgyptQueenCharacterMenu);
        WitchCharacterMenu.SetActive(characterMenu == WitchCharacterMenu);

        // Move your selection sphere, etc.
        //LeanTween.move(selectSphere, spherePosition, 0.2f);
        selectSphere.transform.position = spherePosition;

        // Attach the camera follow
        cameraFollow.target = characterTransform;

        // Save which character is selected in PlayerPrefs
        PlayerPrefs.SetString("SelectedCharacter", characterType);
        PlayerPrefs.Save();

        // Update UI
        RefreshAllCharacterButtons();
        
    }
    public void RefreshAllCharacterButtons()
    {
        // 1) newBoy
        RefreshButtonState(newBoyButton, "newBoy", newBoyCost);

        // 2) newGirl
        RefreshButtonState(newGirlButton, "newGirl", newGirlCost);

        // 3) classic boy
        RefreshButtonState(boyButton, "boy", 0);   // 0 cost => free or auto-bought

        // 4) classic girl
        RefreshButtonState(girlButton, "girl", 0); // same as above

        RefreshButtonState(GwenButton, "Gwen", GwenCost); // same as above

        RefreshButtonState(WitchButton, "Witch", WitchCost); // same as above

        RefreshButtonState(WitchButton, "EgyptQueen", EgyptQueenCost); // same as above
    }


    private void RefreshButtonState(Button button, string characterType, int cost)
    {
        // We find the Text component inside the Button
        Text buttonText = button.GetComponentInChildren<Text>();
        if (buttonText == null) return;

        // Is the character purchased?
        bool purchased = IsCharacterBought(characterType) || cost == 0;

        // Who's selected now?
        string currentSelected = PlayerPrefs.GetString("SelectedCharacter", "boy");

        if (!purchased)
        {
            // Not purchased => show "Cost: XXXX"
            buttonText.text = $"Cost: {cost}";
        }
        else
        {
            // purchased => check if it's currently selected
            if (currentSelected == characterType)
            {
                // show "Selected"
                buttonText.text = "Selected";
            }
            else
            {
                // otherwise "Select"
                buttonText.text = "Select";
            }
        }
    }


    private void LoadSelectedCharacter()
    {
        string selectedCharacterType = PlayerPrefs.GetString("SelectedCharacter", "boy");

        if (selectedCharacterType == "boy")
        {
            SelectCharacter(boyCharacterMenu, boyCharacterGame.transform, "boy", boyPositionTarget.transform.position);
        }
        else if (selectedCharacterType == "girl")
        {
            SelectCharacter(girlCharacterMenu, girlCharacterGame.transform, "girl", girlPositionTarget.transform.position);
        }
        else if (selectedCharacterType == "newGirl")
        {
            SelectCharacter(newGirlCharacterMenu, newGirlCharacterGame.transform, "newGirl", newGirlPositionTarget.transform.position);
        }
        else if (selectedCharacterType == "newBoy")
        {
            SelectCharacter(newBoyCharacterMenu, newBoyCharacterGame.transform, "newBoy", newBoyPositionTarget.transform.position);
        }
        else if (selectedCharacterType == "Gwen")
        {
            SelectCharacter(GwenCharacterMenu, GwenCharacterGame.transform, "Gwen", GwenPositionTarget.transform.position);

        }
        else if (selectedCharacterType == "EgyptQueen")
        {
            SelectCharacter(EgyptQueenCharacterMenu, EgyptQueenCharacterGame.transform, "EgyptQueen", EgyptQueenPositionTarget.transform.position);

        }
        else if (selectedCharacterType == "Witch")
        {
          
            SelectCharacter(WitchCharacterMenu, WitchCharacterGame.transform, "Witch", WitchPositionTarget.transform.position);
        }
        
    }

    private void ResetAssets()
    {
        Assets.GoldPortfolio = 0;
        Assets.StockPortfolio = 0;
        Assets.FixedDepositPortfolio = 0;
        Assets.RealEstatePortfolio = 0;
        Assets.CryptoPortfolio = 0;
        Assets.AntiquePortfolio = 0;
        Assets.MutualFundsPortfolio = 0;
        Assets.LandPortfolio = 0;
    }

    public void ChangeSortOrderToFive()
    {
       
        CharSelectCanvas.sortingOrder = 5;
        
      
    }

    public void ResetPurchasedCharacters()
    {
        PlayerPrefs.DeleteKey("newBoyBought");
        PlayerPrefs.DeleteKey("newGirlBought");
        PlayerPrefs.DeleteKey("GwenBought");
        PlayerPrefs.DeleteKey("WitchBought");
        PlayerPrefs.DeleteKey("EgyptQueenBought");


        // If you also want to reset classic boy/girl (if they have a cost):
        // PlayerPrefs.DeleteKey("boyBought");
        // PlayerPrefs.DeleteKey("girlBought");

        // Reset selected character to "boy" or nothing
        PlayerPrefs.SetString("SelectedCharacter", "boy");

        PlayerPrefs.Save();
        Debug.Log("Character purchases reset!");

        // Then refresh the UI
        RefreshAllCharacterButtons();
    }
    public void RefillGemsTo5000()
    {
        // Set local user gems to 5000
        userGems = 5000;

        // Save in PlayerPrefs (if you’re persisting gems)
        PlayerPrefs.SetInt("UserGems", userGems);
        PlayerPrefs.Save();
        UiManager.Instance.DiamondUpdate(userGems);

        // Update the gemText UI, if assigned
        /*if (gemText != null)
        {
            gemText.text = "Gems: " + userGems;
        }
        if (gemText_InChar_View != null)
            gemText_InChar_View.text = "Gems: " + userGems;*/

        Debug.Log("Gems refilled to 5000!");
    }

}
