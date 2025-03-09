using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highScoreTextGameOver;
    public TextMeshProUGUI scoreTextGamOver;

    private int currentScore = 0;
    private int highScore = 0;
    private bool isGameOver = false; // Add this flag

    private void Start()
    {
        // Load the high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();
    }

    private void Update()
    {
        if (!isGameOver) // Only update the score if the game is not over
        {
            // Update the current score during the game
            // This example assumes score increases with time; adjust as needed
            currentScore = (int)(Time.timeSinceLevelLoad * 10); // Example: 10 points per second
            UpdateScoreUI();
        }
    }

    public void CheckForHighScore()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreUI();
        }
    }

    // New function to update UI when the game is over
    public void UpdateGameOverUI()
    {
        highScoreTextGameOver.text = highScore.ToString();
        scoreTextGamOver.text = currentScore.ToString();
        isGameOver = true; // Set the game over flag
    }

    private void UpdateScoreUI()
    {
        scoreText.text = currentScore.ToString();
    }

    private void UpdateHighScoreUI()
    {
        highScoreText.text = highScore.ToString();
    }

    private void OnDestroy()
    {
        // Save the high score when the game object is destroyed
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
}
